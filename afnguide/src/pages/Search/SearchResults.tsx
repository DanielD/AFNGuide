import { Icon } from "../../components/Icon";
import SearchResultItem from "./SearchResultItem";
import { Pager } from "../../components/Pager";
import { useContext } from "react";
import { SearchSchedulesContext } from "../../modules/SearchSchedulesContext";
import { useNavigate, useLocation } from "react-router-dom";

function SearchResults() {
  const navigate = useNavigate();
  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const { searchResults, pageSize } = useContext(SearchSchedulesContext);

  return (
    <>
      {searchResults && (
        <section className="container g-pt-10 g-pb-45">
          <div className="d-md-flex justify-content-between g-search-filter g-mb-0">
            <h3 className="h6 text-uppercase g-mb-10">
              About&nbsp;
              <span className="g-color-gray-dark-v1">
                {searchResults?.Total ?? 0}
              </span>
              &nbsp;Results&nbsp;
            </h3>
            <ul className="list-inline">
              <li className="list-inline-item g-mr-30">
                <a className="u-link-v5 g-color-gray-dark-v5 g-color-gray-dark-v5--hover">
                  <Icon iconName="List" className="g-pos-rel g-top-1 g-mr-5" />
                  &nbsp;List View
                </a>
              </li>
              <li className="list-inline-item">
                <a className="u-link-v5 g-color-gray-dark-v1 g-color-primary--hover">
                  <Icon iconName="Grid" className="g-pos-rel g-top-1 g-mr-5" />
                  &nbsp;Grid View
                </a>
              </li>
            </ul>
          </div>
          {(searchResults?.Total ?? 0) === 0 && <div>No Records Found</div>}
          {searchResults?.Schedules?.map((result) => (
            <SearchResultItem key={result.Id} item={result} />
          ))}
          <Pager
            currentPage={searchResults?.Page ?? 1}
            pageSize={pageSize}
            totalCount={searchResults?.Total ?? 0}
            siblingCount={1}
            onPageChange={(page: number) => {
              queryParams.set("page", page.toString());
              const newSearch = `?${queryParams.toString()}`;
              navigate({ pathname: "/search", search: newSearch });
            }}
          />
        </section>
      )}
      {!searchResults && <div>Disclaimer Here</div>}
    </>
  );
}

export default SearchResults;
