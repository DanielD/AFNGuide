import { createContext, useState } from "react";
import { SearchSportsResultsModel } from "../models/SearchSportsResultsModel";

export interface SearchSportsSchedulesContextProperties {
  searchResults: SearchSportsResultsModel | null;
  setSearchResults: (results: SearchSportsResultsModel | null) => void;
  pageSize: number;
  setPageSize: (pageSize: number) => void;
}

export const SearchSportsSchedulesContext =
  createContext<SearchSportsSchedulesContextProperties>(null!);

export const SearchSportsSchedulesContextProvider = (props: {
  children: any;
}) => {
  const [searchResults, setSearchResults] = useState(
    null as SearchSportsResultsModel | null
  );
  const [pageSize, setPageSize] = useState(10);

  return (
    <SearchSportsSchedulesContext.Provider
      value={{ searchResults, setSearchResults, pageSize, setPageSize }}
    >
      {props.children}
    </SearchSportsSchedulesContext.Provider>
  );
};
