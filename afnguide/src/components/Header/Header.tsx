import { useContext, useState } from "react";
import {
  Container,
  Nav,
  Navbar,
  NavDropdown,
  Form,
  Button,
  InputGroup,
} from "react-bootstrap";
import { Search as SearchIcon } from "react-bootstrap-icons";
import { useNavigate } from "react-router-dom";

import { MenuItemModel } from "../../models/MenuItemModel";

import { TimeZoneSelector } from "../TimeZoneSelector";

import afn_logo from "../../assets/images/afn_logo.png";

import { AfnContext } from "../../modules/AfnContext";

import "./Header.css";

function Header() {
  const navigate = useNavigate();
  const { topMenuItems } = useContext(AfnContext);
  const [searchText, setSearchText] = useState("");

  function onMenuItemClick(item: MenuItemModel) {
    if (item.Url.startsWith("http")) {
      window.open(item.Url, "_blank");
    } else {
      console.log("navigate to " + item.Url);
    }
  }

  const handleSearch = (event: any = null) => {
    event?.preventDefault();
    if (searchText.trim() === "") return;

    const v = searchText.trim();
    setSearchText("");

    const queryParams = new URLSearchParams();
    queryParams.append("searchText", v);
    const newSearch = `?${queryParams.toString()}`;
    navigate({ pathname: "/search", search: newSearch });
  };

  return (
    <Navbar
      expand="sm"
      className="bg-body-tertiary afn-header"
      data-bs-theme="dark"
    >
      <Container>
        <Navbar.Brand href="/">
          <img src={afn_logo} alt="Home" style={{ height: 33 }} />
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <>
            <Nav className="ms-auto" variant="dark">
              {topMenuItems.map((item, index) => {
                if (item.Items.length === 0) {
                  return (
                    <Nav.Link
                      onClick={() => onMenuItemClick(item)}
                      key={index}
                      className="nav-link"
                    >
                      {item.Text}
                    </Nav.Link>
                  );
                } else {
                  return (
                    <NavDropdown
                      title={item.Text}
                      id="basic-nav-dropdown"
                      key={index}
                    >
                      {item.Items.map((subItem, subIndex) => (
                        <NavDropdown.Item
                          onClick={() => onMenuItemClick(subItem)}
                          key={`${index}-${subIndex}`}
                        >
                          {subItem.Text}
                        </NavDropdown.Item>
                      ))}
                    </NavDropdown>
                  );
                }
              })}
            </Nav>
            <div className="ma-auto">
              <Form>
                <InputGroup className="menu-form-control text-box">
                  <Form.Control
                    type="search"
                    placeholder="Search TV Shows, Movies, and more..."
                    className="mr-sm-2"
                    aria-label="Search TV Shows, Movies, and more..."
                    aria-describedby="header-button-search"
                    value={searchText}
                    onChange={(e) => setSearchText(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        handleSearch();
                      }
                    }}
                  />
                  <Button
                    type="submit"
                    variant="outline-secondary"
                    id="header-button-search"
                    title="Search"
                    onClick={(e) => handleSearch(e)}
                  >
                    <SearchIcon color="#fff" />
                  </Button>
                </InputGroup>
              </Form>
            </div>
            <TimeZoneSelector className="menu-form-control timezone-selector" />
          </>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default Header;
