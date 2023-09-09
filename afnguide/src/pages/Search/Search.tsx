import SearchForm from "./SearchForm";
import { SubHeader } from "../../components/SubHeader";
import SearchResults from "./SearchResults";
import { SearchSchedulesContextProvider } from "../../modules/SearchSchedulesContext";

function Search() {
  return (
    <>
      <SubHeader
        title="Search Schedules"
        crumbs={["Home", "TV", "Search All"]}
      />
      <SearchSchedulesContextProvider>
        <SearchForm />
        <SearchResults />
      </SearchSchedulesContextProvider>
    </>
  );
}

export default Search;
