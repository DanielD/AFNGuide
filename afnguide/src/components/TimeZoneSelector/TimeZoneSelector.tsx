import React, { useState, forwardRef, useRef, useContext } from "react";
import { useCookies } from "react-cookie";
import { Form, Dropdown, Container, Col, Row } from "react-bootstrap";
import { Globe2 as GlobeIcon } from "react-bootstrap-icons";
import Select, { createFilter } from "react-select";

import { AfnContext } from "../../modules/AfnContext";

import { TimeZoneModel } from "../../models/TimeZoneModel";

import "./TimeZoneSelector.css";

function TimeZoneSelector(props?: { className?: string }) {
  const { timeZones } = useContext(AfnContext);
  const [cookies, setCookie] = useCookies(["timezone"]);
  const [timeZoneDisplay, setTimeZoneDisplay] = useState("");
  const dropDownRef = useRef<any>(null);

  function handleChange(e: any) {
    const tz = e as TimeZoneModel;
    setCookie("timezone", tz.Id, { path: "/", expires: new Date(2030, 1, 1) });
    setTimeZoneDisplay(tz.Name);

    //dropDownRef.current.toggle();
  }

  function getTimeZone() {
    const tz = timeZones.find((tz) => tz.Id === parseInt(cookies.timezone));
    return tz ? tz.Name : "";
  }

  return (
    <Dropdown
      autoClose="outside"
      ref={dropDownRef}
      className={props?.className}
      data-bs-theme="dark"
    >
      <Dropdown.Toggle
        variant="outline-secondary"
        id="dropdown-basic"
        title={timeZoneDisplay || getTimeZone()}
      >
        <GlobeIcon color="#fff" />
      </Dropdown.Toggle>
      <Dropdown.Menu>
        <Form>
          <Container>
            <Col>
              <Row>
                <Select
                  isSearchable
                  options={timeZones}
                  getOptionLabel={(tz) => tz.Name}
                  getOptionValue={(tz) => `${tz.Id}`}
                  value={timeZones.find(
                    (tz) => tz.Id === parseInt(cookies.timezone)
                  )}
                  onChange={handleChange}
                  filterOption={createFilter({ matchFrom: "any" })}
                  theme={(theme) => ({
                    ...theme,
                    colors: {
                      ...theme.colors,
                      neutral0: "#212529",
                      primary: "#fff",
                      primary25: "#0000003",
                    },
                  })}
                />
              </Row>
            </Col>
          </Container>
        </Form>
      </Dropdown.Menu>
    </Dropdown>
  );
}

export default TimeZoneSelector;
