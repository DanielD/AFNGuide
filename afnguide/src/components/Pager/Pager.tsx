import { Icon } from "../Icon";
import { usePagination } from "./usePagination";

function Pager(props: {
  currentPage: number;
  totalCount: number;
  pageSize: number;
  siblingCount: number;
  onPageChange: (page: number) => void;
}) {
  const paginationRange = usePagination({
    currentPage: props.currentPage,
    totalCount: props.totalCount,
    pageSize: props.pageSize,
    siblingCount: props.siblingCount,
  });

  const DOTS = "...";

  // If there are less than 2 times in pagination range we shall not render the component
  if (props.currentPage === 0 || paginationRange!.length < 2) {
    return null;
  }

  const onNext = () => {
    props.onPageChange(props.currentPage + 1);
  };

  const onPrevious = () => {
    props.onPageChange(props.currentPage - 1);
  };

  let lastPage = paginationRange![paginationRange!.length - 1];

  return (
    <nav className="g-mb-30 g-mx-15" aria-label="Page Navigation">
      <ul className="list-inline">
        {/* Left navigation arrow */}
        <li className="list-inline-item" key={"pager-1"} data-key={"pager-1"}>
          <button
            type="button"
            className="btn btn-sm u-btn-primary g-pa-4-11"
            onClick={() => onPrevious()}
            aria-label="Previous"
            disabled={props.currentPage === 1}
          >
            <span aria-hidden="true">
              <Icon iconName="ArrowLeft" />
              &nbsp;
            </span>
            <span className="sr-only">Previous</span>
          </button>
        </li>
        {paginationRange!.map((pageNumber, index) => {
          // If the pageItem is a DOT, render the DOTS unicode character
          if (pageNumber === DOTS) {
            return (
              <li
                className="list-inline-item g-hidden-sm-down"
                key={"pager" + index}
                data-key={"pager" + index}
              >
                <span className="g-pa-4-11">&#8230;</span>
              </li>
            );
          }

          // Render our Page Pills
          return (
            <li
              className="list-inline-item g-hidden-sm-down"
              key={"pager" + index}
              data-key={"pager" + index}
            >
              <button
                type="button"
                className="btn btn-sm u-btn-primary g-pa-4-11"
                onClick={() =>
                  props.onPageChange(parseInt(pageNumber.toString()))
                }
                disabled={props.currentPage === pageNumber}
              >
                {pageNumber}
              </button>
            </li>
          );
        })}
        {/* Right navigation arrow */}
        <li
          className="list-inline-item"
          key={"pager" + paginationRange!.length}
          data-key={"pager" + paginationRange!.length}
        >
          <button
            type="button"
            className="btn btn-sm u-btn-primary g-pa-4-11"
            onClick={() => onNext()}
            aria-label="Previous"
            disabled={props.currentPage === lastPage}
          >
            <span className="sr-only">Next</span>
            <span aria-hidden="true">
              &nbsp;
              <Icon iconName="ArrowRight" />
            </span>
          </button>
        </li>
        <li
          className="list-inline-item float-right"
          key={"pager" + (paginationRange!.length + 1)}
          data-key={"pager" + (paginationRange!.length + 1)}
        >
          <span className="u-pagination-v1__item-info g-pa-4-11">
            Page {props.currentPage} of {lastPage}
          </span>
        </li>
      </ul>
    </nav>
  );
}

export default Pager;
