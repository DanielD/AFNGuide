import { useContext } from "react";
import { Container, Row, Col } from "react-bootstrap";

import { Icon } from "../Icon";

import AboutUs from "./AboutUs";
import LatestMoviesAndShows from "./LatestMoviesAndShows";
import Links from "./Links";
import OurContacts from "./OurContacts";
import SocialMediaIcons from "./SocialMediaIcons";

function Footer() {
  return (
    <>
      <div className="afn-bg-dark g-pt-50 g-pb-25">
        <Container>
          <Row>
            <Col lg="3" md="6" sm="6" className="col-12 g-mb-25 g-mb-0--lg">
              <AboutUs />
            </Col>
            <Col lg="3" md="6" sm="6" className="col-12 g-mb-25 g-mb-0--lg">
              <LatestMoviesAndShows />
            </Col>
            <Col lg="3" md="6" sm="6" className="col-12 g-mb-25 g-mb-0--lg">
              <Links />
            </Col>
            <Col lg="3" md="6" sm="6" className="col-12 g-mb-25 g-mb-0--lg">
              <OurContacts />
            </Col>
          </Row>
        </Container>
      </div>
      <footer className="g-bg-gray-dark-v1 g-color-white-opacity-0_8 g-py-20">
        <Container>
          <Row>
            <Col md="8" className="text-center text-md-left g-mb-15 g-mb-0--md">
              <div className="d-lg-flex">
                <small className="d-block g-font-size-default g-mr-10 g-mb-10 g-mb-0--md">
                  &copy; 2023 - All Rights Reserved
                </small>
              </div>
            </Col>
            <Col md="4" className="align-self-center">
              <SocialMediaIcons />
            </Col>
          </Row>
        </Container>
      </footer>
      <a
        className="js-go-to u-go-to-v1"
        href="#"
        data-type="fixed"
        data-position='{"bottom": 15, "right": 15}'
        data-offset-top="400"
        data-compensation="#js-header"
        data-show-effect="zoomIn"
      >
        <Icon iconName="ArrowUpShort" />
      </a>
    </>
  );
}

export default Footer;
