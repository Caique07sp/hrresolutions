(function (angular) {
    "use strict";

    angular
        .module("todoApp")
        .directive("todoPaginatedList", [todoPaginatedList])
        .directive("pagination", [pagination]);

    /**
     * <todo-paginated-list></todo-paginated-list>
     *
     * Self-contained list (isolated scope, no attributes needed): it owns the paging/sorting state, 
     * asks the server for ONE page at a time (paging and sorting happen on the server, so they apply to the 
     * whole list) and feeds the reusable <pagination> component.
     *
     * Server contract: GET api/Todo/Todos?page=&pageSize=&sortBy=&sortDir=
     *                  -> { items: [], page, pageSize, totalItems, totalPages }
     */
    function todoPaginatedList() {
        var directive = {
            restrict: "E",
            templateUrl: "app/templates/todo.list.paginated.html",
            scope: {},
            controller: ["$scope", "$http", controller]
        };

        function controller($scope, $http) {
            var DEFAULT_PAGE_SIZE = 20;
            // what the server uses (and what we ask for) while the user has not clicked any header
            var DEFAULT_SORT = { property: "createdDate", descending: true };
            var ASC = "\u25B2", DESC = "\u25BC";
            var lastRequest = 0; // id of the latest request: older responses are ignored (see load)

            $scope.todos = [];
            $scope.loading = false;
            $scope.error = null;
            // bound to the pagination component
            $scope.paging = { page: 1, pageSize: DEFAULT_PAGE_SIZE, totalItems: 0 };
            // property === null -> default order, and no indicator on the headers
            $scope.sort = { property: null, descending: false };

            $scope.load = load;
            $scope.sortBy = sortBy;
            $scope.sortIndicator = sortIndicator;

            load(1, DEFAULT_PAGE_SIZE);

            /** Fetches one page, keeping the current sort. Called by the pagination component on every change. */
            function load(page, pageSize) {
                var request = ++lastRequest;
                var sort = $scope.sort.property ? $scope.sort : DEFAULT_SORT;

                $scope.loading = true;
                $scope.error = null;

                $http.get("api/Todo/Todos", {
                    params: {
                        page: page,
                        pageSize: pageSize,
                        sortBy: sort.property,
                        sortDir: sort.descending ? "desc" : "asc"
                    }
                }).then(function (response) {
                    // the user may have clicked again while this was in flight: drop stale answers
                    if (request !== lastRequest) { return; }
                    var data = response.data;
                    $scope.todos = data.items;
                    // trust the server's effective values (e.g. a page past the end gets clamped)
                    $scope.paging.page = data.page;
                    $scope.paging.pageSize = data.pageSize;
                    $scope.paging.totalItems = data.totalItems;
                }, function () {
                    if (request !== lastRequest) { return; }
                    $scope.todos = [];
                    $scope.error = "Could not load the list. Please try again.";
                })["finally"](function () {
                    if (request === lastRequest) { $scope.loading = false; }
                });
            }

            /** Header click: 1st click ascending, next clicks toggle; a new property always starts ascending. */
            function sortBy(property) {
                if ($scope.sort.property === property) {
                    $scope.sort.descending = !$scope.sort.descending;
                } else {
                    $scope.sort.property = property;
                    $scope.sort.descending = false;
                }
                // back to the first page, keeping the selected page size
                load(1, $scope.paging.pageSize);
            }

            function sortIndicator(property) {
                if ($scope.sort.property !== property) { return ""; }
                return $scope.sort.descending ? DESC : ASC;
            }
        }

        return directive;
    }

    /**
     * <pagination page="..." page-size="..." total-items="..." on-change="fn(page, pageSize)"></pagination>
     *
     * Reusable: knows nothing about Todos, only about numbers.
     *   page         (=) current 1-based page; the component updates it
     *   page-size    (=) items per page, 0 = all; the component updates it
     *   total-items  (=) total number of items in the whole list (read only)
     *   on-change    (&) called AFTER the user changed page or page size, with the NEW values as the
     *                    locals `page` and `pageSize` (the parent should fetch the data there).
     *                    It is passed as arguments because the two-way bindings only reach the parent 
     *                    on the next digest, so reading the parent's fields inside the callback could be stale.
     */
    function pagination() {
        var directive = {
            restrict: "E",
            templateUrl: "app/templates/pagination.html",
            scope: {
                page: "=",
                pageSize: "=",
                totalItems: "=",
                onChange: "&"
            },
            controller: ["$scope", controller]
        };

        function controller($scope) {
            $scope.pageSizeOptions = [
                { label: "10", value: 10 },
                { label: "20", value: 20 },
                { label: "30", value: 30 },
                { label: "all", value: 0 }
            ];
            // model of the 'go to page' input: kept apart from `page` so that half-typed values don't trigger requests
            $scope.input = { page: $scope.page };

            $scope.totalPages = totalPages;
            $scope.goTo = goTo;
            $scope.goToInput = goToInput;
            $scope.resetInput = resetInput;
            $scope.changePageSize = changePageSize;

            // the parent may change `page` too (e.g. the server clamped it): keep the input in sync
            $scope.$watch("page", function (page) { $scope.input.page = page; });

            function totalPages() {
                if (!$scope.pageSize) { return 1; } // 0 = all
                return Math.max(1, Math.ceil(($scope.totalItems || 0) / $scope.pageSize));
            }

            function goTo(page) {
                page = Math.min(Math.max(page, 1), totalPages());
                $scope.input.page = page;
                if (page === $scope.page) { return; }
                $scope.page = page;
                notify();
            }

            /** The input's value changed (debounced): clamp it into [1, totalPages]. */
            function goToInput() {
                var page = parseInt($scope.input.page, 10);
                if (isNaN(page)) { return; } // empty/invalid while typing: wait, resetInput() fixes it on blur
                goTo(page);
            }

            function resetInput() { $scope.input.page = $scope.page; }

            function changePageSize() {
                $scope.page = 1; // the old page number means nothing with a different page size
                notify();
            }

            function notify() {
                $scope.onChange({ page: $scope.page, pageSize: $scope.pageSize });
            }
        }

        return directive;
    }

})(angular);
