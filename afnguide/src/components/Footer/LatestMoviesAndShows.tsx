import { useContext } from "react";
import { AfnContext } from "../../modules/AfnContext";
import { Col, Row } from "react-bootstrap";

export default function LatestMoviesAndShows() {
  const { promoAItems } = useContext(AfnContext);
  const filteredItems = promoAItems
    .filter((item) => item.Title !== undefined)
    .map((item) => item);

  return (
    <>
      <div className="u-heading-v3-1 g-brd-white-opacity-0_3 g-mb-25">
        <h2 className="u-heading-v3__title h6 text-uppercase g-brd-white">
          Latest Movies &amp; Shows
        </h2>
      </div>
      {filteredItems.map((item, index) => (
        <article key={index}>
          <h3 className="h6 g-mb-8">
            <a
              className="g-color-white-opacity-0_8 g-color-white--hover"
              href={item.LinkUrl ?? "#"}
              target="_blank"
            >
              {item.Title}
            </a>
            {item.Schedules!.length > 0 && (
              <Row>
                <div className="small g-color-white-opacity-0_6 g-width-50x">
                  {item.Schedules![0].Date}
                </div>
                <div className="small g-color-white-opacity-0_6 g-width-50x" style={{textAlign: "end"}}>
                  {item.Schedules![0].Time}
                </div>
              </Row>
            )}
          </h3>
        </article>
      ))}
    </>
  );
}
