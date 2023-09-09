import { useContext } from "react";
import { Row, Col, Container } from "react-bootstrap";

import { AfnContext } from "../../../modules/AfnContext";

import "./AfnInfo.css";

function AfnInfo() {
  const { bulletins } = useContext(AfnContext);

  const cleanContent = (content: string | undefined) => {
    return content?.replace(/(<([^>]+)>)/gi, "").substring(0, 100) + "...";
  }

  return (
    <>
      <div className="text-center">
        <h2 className="h3 g-color-black text-uppercase mb-2">AFN Info</h2>
        <div className="d-inline-block g-width-35 g-height-2 g-bg-primary mb-2"></div>
      </div>
      <Container>
        <Row>
          {bulletins.map((bulletin, index) => {
            return (<Col lg={6} className="g-mb-30" key={index}>
              <div className="info-box-shadow afn-shadow-dark-10 afn-bg-light-extra rounded g-pb-30 g-px-30">
                <div className="media g-mb-0">
                  <div className="media-body">
                    <h3 className="h5 g-color-black mb-20">{bulletin.Title}</h3>
                    <div className="g-width-30 g-brd-bottom afn-brd-color-dark g-my-15"></div>
                    <p className="afn-text-color-dark g-mb-0">{cleanContent(bulletin.Content)}</p>
                    <a href={"/Bulletins#" + bulletin.Id} className="btn u-btn-primary afn-bg-dark g-font-size-12 text-uppercase g-py-12 g-px-25 g-mt-20">Read More</a>
                  </div>
                </div>
              </div>
            </Col>)
          })}
        </Row>
      </Container>
    </>
  );
}

export default AfnInfo;
