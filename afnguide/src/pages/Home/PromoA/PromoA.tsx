import { useContext } from "react";
import { AfnContext } from "../../../modules/AfnContext";
import { Carousel } from "react-bootstrap";

import { PromoModel } from "../../../models/PromoModel";

import "./PromoA.css";

function PromoA() {
  const { promoAItems } = useContext(AfnContext);

  function goToUrl(model: PromoModel) {
    if (model.LinkUrl) {
      window.open(model.LinkUrl, "_blank");
    }
  }

  return (
    <Carousel indicators={false}>
      {promoAItems.map((item, index) => (
        <Carousel.Item key={index} interval={5000}>
          <img
            className="d-block w-100"
            src={item.ImageUrl}
            alt={item.Title}
            onClick={() => goToUrl(item)}
          />
        </Carousel.Item>
      ))}
    </Carousel>
  );
}

export default PromoA;
