import { createContext, useState } from "react";
import { SearchResultsModel } from "../models/SearchResultsModel";

export interface SearchSchedulesContextProperties {
  searchResults: SearchResultsModel | null;
  setSearchResults: (results: SearchResultsModel | null) => void;
  pageSize: number;
  setPageSize: (pageSize: number) => void;
}

export const SearchSchedulesContext =
  createContext<SearchSchedulesContextProperties>(null!);

export const SearchSchedulesContextProvider = (props: { children: any }) => {
  const [searchResults, setSearchResults] = useState(
    null as SearchResultsModel | null
  );
  const [pageSize, setPageSize] = useState(10);

  return (
    <SearchSchedulesContext.Provider
      value={{ searchResults, setSearchResults, pageSize, setPageSize }}
    >
      {props.children}
    </SearchSchedulesContext.Provider>
  );
};
