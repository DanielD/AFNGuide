import {
  Container,
  Row,
  Col,
  Form,
  InputGroup,
  Button,
  Collapse,
} from "react-bootstrap";
import { Icon } from "../../components/Icon";
import { useEffect, useState, useContext } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import queryString from "query-string";
import { useCookies } from "react-cookie";
import { usePreviousValue } from "../../hooks/usePreviousValue";
import {
  SearchResultsModel,
  SearchResultItemModel,
} from "../../models/SearchResultsModel";
import { SearchSchedulesContext } from "../../modules/SearchSchedulesContext";
import { AfnContext } from "../../modules/AfnContext";

const getBoolean = (value: boolean | undefined, defaultValue: boolean) => {
  if (value === undefined) return defaultValue;
  return value;
};

function SearchForm() {
  const navigate = useNavigate();
  const location = useLocation();
  let queryParams = new URLSearchParams(location.search);
  const query = queryString.parse(location.search, {
    // arrayFormat: "bracket-separator",
    // arrayFormatSeparator: ",",
    parseNumbers: true,
  });
  const { setSearchResults, setPageSize } = useContext(SearchSchedulesContext);
  const { channels } = useContext(AfnContext);
  const [cookies] = useCookies(["timezone"]);
  const [open, setOpen] = useState(false);

  const [usePrimeAtlantic, setPrimeAtlantic] = useState(
    getBoolean((query.channels as Array<number>)?.includes(2), true)
  );
  const [usePrimePacific, setPrimePacific] = useState(
    getBoolean((query.channels as Array<number>)?.includes(4), true)
  );
  const [useFamily, setFamily] = useState(
    getBoolean((query.channels as Array<number>)?.includes(8), true)
  );
  const [useNews, setNews] = useState(
    getBoolean((query.channels as Array<number>)?.includes(5), true)
  );
  const [useSpectrum, setSpectrum] = useState(
    getBoolean((query.channels as Array<number>)?.includes(3), true)
  );
  const [useMovie, setMovie] = useState(
    getBoolean((query.channels as Array<number>)?.includes(9), true)
  );
  const [useSports, setSports] = useState(
    getBoolean((query.channels as Array<number>)?.includes(1), true)
  );
  const [useSports2, setSports2] = useState(
    getBoolean((query.channels as Array<number>)?.includes(6), true)
  );
  const [dateStart, setDateStart] = useState((query.startDate as string) || "");
  const [dateEnd, setDateEnd] = useState((query.endDate as string) || "");
  const [perPage, setPerPage] = useState((query.perPage as number) || 10);
  const [page] = useState((query.page as number) || 1);
  const [rating, setRating] = useState((query.rating as string) || "Any");
  const [searchField, setSearchField] = useState(
    (query.searchField as string) || "All"
  );
  const [searchText, setSearchText] = useState(
    (query.searchText as string) || ""
  );
  const [searchPhrase, setSearchPhrase] = useState(
    (query.searchPhrase as string) || ""
  );
  const [searchUnwanted, setSearchUnwanted] = useState(
    (query.searchUnwanted as string) || ""
  );

  const queryValue = usePreviousValue(query);
  const [hasPageLoaded, setHasPageLoaded] = useState(false);
  const [isSearchDisabled, setIsSearchDisabled] = useState(false);

  useEffect(() => {
    const checkChannel = (
      channels: Array<number>,
      channel: number,
      channelState: boolean
    ) => {
      return channels.includes(channel) === channelState;
    };

    const checkQueryValue = () => {
      if (queryValue === undefined) return false;

      const qv = queryValue as any;

      if (qv.endDate !== dateEnd) return false;
      if (parseInt(qv.page) !== page) return false;
      if (parseInt(qv.perPage) !== perPage) return false;
      if (qv.rating !== rating) return false;
      if (qv.searchField !== searchField) return false;
      if (qv.searchPhrase !== searchPhrase) return false;
      if (qv.searchText !== searchText) return false;
      if (qv.searchUnwanted !== searchUnwanted) return false;
      if (qv.startDate !== dateStart) return false;

      if (qv.channels) {
        if (!checkChannel(qv.channels, 2, usePrimeAtlantic)) return false;
        if (!checkChannel(qv.channels, 4, usePrimePacific)) return false;
        if (!checkChannel(qv.channels, 8, useFamily)) return false;
        if (!checkChannel(qv.channels, 5, useNews)) return false;
        if (!checkChannel(qv.channels, 3, useSpectrum)) return false;
        if (!checkChannel(qv.channels, 9, useMovie)) return false;
        if (!checkChannel(qv.channels, 1, useSports)) return false;
        if (!checkChannel(qv.channels, 6, useSports2)) return false;
      }

      return true;
    };

    const blnCheckQueryValue = checkQueryValue();

    setIsSearchDisabled(blnCheckQueryValue);

    // if (blnCheckQueryValue) {
    //   return;
    // } else {
    //   setHasPageLoaded(false);
    // }

    // if (hasPageLoaded) {
    //   return;
    // }

    const searchSchedules = () => {
      if (
        (!query.searchText || query.searchText === "") &&
        (!query.searchPhrase || query.searchPhrase === "")
      )
        return;
      if (channels === undefined || channels.length === 0) return;

      const searchTextValues = (query.searchText as string).match(
        /(".*?"|[^"\s]+)(.*?|[^\s]+)(?=\s|\s$|$)/g
      );
      const unwantedWordsValues = query.searchUnwanted
        ? (query.searchUnwanted as string).match(
            /(".*?"|[^"\s]+)(.*?|[^\s]+)(?=\s|\s$|$)/g
          )
        : [];

      let url = `/api/Schedule/search?tz=${cookies.timezone ?? "1"}&`;
      if (query.channels) {
        (query.channels as Array<number>).forEach((channel: number) => {
          url += `ch=${channel}&`;
        });
      }
      if (query.startDate) {
        url += `sdt=${query.startDate}&`;
      }
      if (query.endDate) {
        url += `edt=${query.endDate}&`;
      }
      if (query.perPage) {
        url += `sz=${query.perPage}&`;
      }
      if (query.page) {
        url += `p=${query.page}&`;
      }
      if (query.rating) {
        url += `r=${query.rating}&`;
      }
      if (query.searchField) {
        url += `f=${query.searchField}&`;
      }
      if (searchTextValues) {
        searchTextValues.forEach((searchTextValue: string) => {
          url += `q=${searchTextValue}&`;
        });
      }
      if (query.searchPhrase) {
        url += `ph=${query.searchPhrase}&`;
      }
      if (unwantedWordsValues) {
        unwantedWordsValues.forEach((unwantedWordsValue: string) => {
          url += `uw=${unwantedWordsValue}&`;
        });
      }

      console.log(url);

      fetch(url, {})
        .then((response) => response.json())
        .then((data) => {
          const searchResults = new SearchResultsModel(
            data.page,
            data.total,
            data.schedules?.map((schedule: any) => {
              return new SearchResultItemModel(
                schedule.id,
                schedule.title,
                schedule.channelId,
                schedule.isPremiere,
                schedule.airDateLocal,
                schedule.episodeTitle,
                schedule.description,
                schedule.duration,
                schedule.rating,
                schedule.genre,
                schedule.year
              );
            })
          );

          setSearchResults(searchResults);
        })
        .catch((error) => console.log(error));
    };

    searchSchedules();
  }, [
    queryValue,
    cookies.timezone,
    query.channels,
    query.endDate,
    query.page,
    query.perPage,
    query.rating,
    query.searchField,
    query.searchPhrase,
    query.searchText,
    query.searchUnwanted,
    query.startDate,
    usePrimeAtlantic,
    usePrimePacific,
    useFamily,
    useNews,
    useSpectrum,
    useMovie,
    useSports,
    useSports2,
    dateEnd,
    dateStart,
    perPage,
    page,
    rating,
    searchField,
    searchText,
    searchPhrase,
    searchUnwanted,
    hasPageLoaded,
    setSearchResults,
    channels,
  ]);

  const handleSearch = (e: any = null) => {
    e?.preventDefault();
    if (!isSearchDisabled) {
      setHasPageLoaded(false);
      setPageSize(perPage);
      queryParams = new URLSearchParams();
      queryParams.set("searchText", searchText);
      const newSearch = `?${queryParams.toString()}`;
      navigate({ pathname: "/search", search: newSearch });
    }
  };

  const handleAdvancedSearch = (e: any = null) => {
    e?.preventDefault();
    if (!isSearchDisabled) {
      setHasPageLoaded(false);
      setPageSize(perPage);

      queryParams.set("searchText", searchText);
      queryParams.set("page", page.toString());
      queryParams.set("perPage", perPage.toString());
      queryParams.set("rating", rating);
      queryParams.set("searchField", searchField);
      queryParams.set("searchPhrase", searchPhrase);
      queryParams.set("searchUnwanted", searchUnwanted);
      queryParams.set("startDate", dateStart);
      queryParams.set("endDate", dateEnd);

      const selectedChannels = [];
      if (usePrimeAtlantic) selectedChannels.push(2);
      if (usePrimePacific) selectedChannels.push(4);
      if (useFamily) selectedChannels.push(8);
      if (useNews) selectedChannels.push(5);
      if (useSpectrum) selectedChannels.push(3);
      if (useMovie) selectedChannels.push(9);
      if (useSports) selectedChannels.push(1);
      if (useSports2) selectedChannels.push(6);

      queryParams.delete("channels");

      selectedChannels.forEach((channel: number) => {
        queryParams.append("channels", channel.toString());
      });

      const newSearch = `?${queryParams.toString()}`;
      navigate({ pathname: "/search", search: newSearch });
    }
  };

  return (
    <section className="g-pt-75 g-pb-45">
      <Container>
        <Row>
          <Col lg={12} className="gmb-0">
            <Form>
              <InputGroup className="text-box">
                <Form.Control
                  type="search"
                  placeholder="Search TV Shows, Movies, and more..."
                  value={searchText}
                  onChange={(e) => setSearchText(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      if (isSearchDisabled) return;

                      handleSearch();
                    }
                  }}
                />
                <Button
                  disabled={isSearchDisabled}
                  type="button"
                  variant="primary"
                  title="Search"
                  className="afn-bg-dark"
                  onClick={handleSearch}
                >
                  Search
                </Button>
              </InputGroup>
              <Button onClick={() => setOpen(!open)} variant="link">
                Advanced Search with Filters&nbsp;
                <Icon iconName="ChevronDown" />
              </Button>
              <Collapse in={open}>
                <Container>
                  <Row>
                    <Col md={10} className="card card-body">
                      {channels && (
                        <Row>
                          {channels.map((channel) => {
                            function setChannelChecked(
                              isChecked: boolean
                            ): void {
                              switch (channel.Id) {
                                case 2:
                                  setPrimeAtlantic(isChecked);
                                  break;
                                case 4:
                                  setPrimePacific(isChecked);
                                  break;
                                case 8:
                                  setFamily(isChecked);
                                  break;
                                case 5:
                                  setNews(isChecked);
                                  break;
                                case 3:
                                  setSpectrum(isChecked);
                                  break;
                                case 9:
                                  setMovie(isChecked);
                                  break;
                                case 1:
                                  setSports(isChecked);
                                  break;
                                case 6:
                                  setSports2(isChecked);
                                  break;
                              }
                            }

                            function getChannelChecked(): boolean {
                              switch (channel.Id) {
                                case 2:
                                  return usePrimeAtlantic;
                                case 4:
                                  return usePrimePacific;
                                case 8:
                                  return useFamily;
                                case 5:
                                  return useNews;
                                case 3:
                                  return useSpectrum;
                                case 9:
                                  return useMovie;
                                case 1:
                                  return useSports;
                                case 6:
                                  return useSports2;
                                default:
                                  return false;
                              }
                            }

                            return (
                              <Col md={2} key={channel.Id}>
                                <Form.Check
                                  key={channel.Id}
                                  type="checkbox"
                                  label={channel.Title}
                                  checked={getChannelChecked()}
                                  onChange={(e) =>
                                    setChannelChecked(e.target.checked)
                                  }
                                />
                              </Col>
                            );
                          })}
                        </Row>
                      )}
                    </Col>
                  </Row>
                  <Row>
                    <Col md={4} className="card card-body g-mt-5 g-mr-5">
                      <Form.Group>
                        <Form.Label htmlFor="perPage2">
                          Records per Page
                        </Form.Label>
                        <Form.Select
                          id="perPage2"
                          value={perPage}
                          onChange={(e) => setPerPage(parseInt(e.target.value))}
                        >
                          <option>5</option>
                          <option>10</option>
                          <option>25</option>
                          <option>50</option>
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={4} className="card card-body g-mt-5 g-mr-5">
                      <Form.Group>
                        <Form.Label htmlFor="dateFrom">Date Range</Form.Label>
                        <div className="justify-content-between d-flex">
                          <Form.Control
                            type="date"
                            id="dateFrom"
                            value={dateStart}
                            onChange={(e) => setDateStart(e.target.value)}
                          />
                          &nbsp;to&nbsp;
                          <Form.Control
                            type="date"
                            id="dateTo"
                            value={dateEnd}
                            onChange={(e) => setDateEnd(e.target.value)}
                          />
                        </div>
                      </Form.Group>
                    </Col>
                    <Col md={3} className="card card-body g-mt-5">
                      <Form.Group>
                        <Form.Label htmlFor="rating">Rating</Form.Label>
                        <Form.Select
                          id="rating"
                          value={rating}
                          onChange={(e) => setRating(e.target.value)}
                        >
                          <option>Any</option>
                          <option>TV-Y</option>
                          <option>TV-Y7</option>
                          <option>TV-G</option>
                          <option>TV-PG</option>
                          <option>TV-14</option>
                          <option>TV-MA</option>
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={5} className="card card-body g-mt-5">
                      <Form.Group>
                        <Form.Label htmlFor="searchField">
                          Search Field
                        </Form.Label>
                        <Form.Select
                          id="searchField"
                          value={searchField}
                          onChange={(e) => setSearchField(e.target.value)}
                        >
                          <option>All</option>
                          <option>Title</option>
                          <option>Episode Title</option>
                          <option>Description</option>
                          <option>Genre</option>
                        </Form.Select>
                      </Form.Group>
                      <Form.Group>
                        <Form.Control
                          type="text"
                          className="g-mt-5"
                          placeholder="Search Text"
                          value={searchText}
                          onChange={(e) => setSearchText(e.target.value)}
                        />
                      </Form.Group>
                      <Form.Group>
                        <Form.Control
                          type="text"
                          className="g-mt-5"
                          placeholder="Search Phrase"
                          value={searchPhrase}
                          onChange={(e) => setSearchPhrase(e.target.value)}
                        />
                      </Form.Group>
                      <Form.Group>
                        <Form.Control
                          type="text"
                          className="g-mt-5"
                          placeholder="Unwanted Words"
                          value={searchUnwanted}
                          onChange={(e) => setSearchUnwanted(e.target.value)}
                        />
                      </Form.Group>
                    </Col>
                    <Col
                      md={12}
                      className="g-mt-5 g-px-0 d-flex justify-content-end"
                    >
                      <Button
                        disabled={isSearchDisabled}
                        type="button"
                        variant="primary"
                        title="Search"
                        className="afn-bg-dark"
                        onClick={handleAdvancedSearch}
                      >
                        Search
                      </Button>
                    </Col>
                  </Row>
                </Container>
              </Collapse>
            </Form>
          </Col>
        </Row>
      </Container>
    </section>
  );
}

export default SearchForm;
