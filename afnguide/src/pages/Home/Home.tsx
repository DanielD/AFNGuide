import { Container, Col, Row } from "react-bootstrap";
import { VerticalMenu } from "./VerticalMenu";
import { PromoA } from "./PromoA";
import { PromoB } from "./PromoB";
import { AfnInfo } from "./AfnInfo";

import "./Home.css";

function Home() {
  return (
    <>
      <section className="afn-bg-light">
        <Container>
          <Row className="justify-content-between g-pt-20 g-pb-20">
            <Col lg={6} className="g-mb-0">
              <VerticalMenu menuKey="AFN TV" afnTitle="TV" />
              <VerticalMenu menuKey="AFN RADIO" afnTitle="RADIO" />
              <VerticalMenu
                menuKey="OTHER"
                className="g-mb-60--lg g-mb-20--md"
              />
            </Col>
            <Col lg={6} className="align-self-center g-overflow-hidden">
              <PromoA />
            </Col>
          </Row>
        </Container>
      </section>
      <section className="afn-bg-light-extra">
        <Container>
          <Row className="justify-content-between g-pt-20 g-pb-20">
            <Col lg={4} className="align-self-center g-overflow-hidden">
              <PromoB />
            </Col>
            <Col lg={8} className="g-mb-0">
              <AfnInfo />
            </Col>
          </Row>
        </Container>
      </section>
    </>
  );
}

export default Home;
