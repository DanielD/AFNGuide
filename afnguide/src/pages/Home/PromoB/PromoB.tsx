import { useContext } from "react";

import { AfnContext } from "../../../modules/AfnContext";
import { PromoModel } from "../../../models/PromoModel";

import "./PromoB.css";

const PromoBImage = (props: { model: PromoModel }): JSX.Element => {
  return (
    <div
      className="promoBContainer"
      onClick={() => goToUrl(props.model)}
      style={{ cursor: props.model.LinkUrl ? "pointer" : "auto" }}
      title={props.model.LinkUrl ? `Go to: ${props.model.LinkUrl}` : ""}
    >
      <img
        className="img-fluid"
        src={props.model.ImageUrl}
        alt={props.model.LinkUrl ? `Go to: ${props.model.LinkUrl}` : ""}
      />
    </div>
  );
};

function goToUrl(model: PromoModel) {
  if (model.LinkUrl) {
    window.open(model.LinkUrl, "_blank");
  }
}

function PromoB() {
  const { promoBItems } = useContext(AfnContext);

  let promoBWrapper;
  if (promoBItems.length === 1) {
    const model = promoBItems[0];
    promoBWrapper = (
      <div className="promoBSingle">
        <PromoBImage model={model} />
      </div>
    );
  } else {
    promoBWrapper = (
      <div className="promoBDualRows">
        {promoBItems.map((model, index) => (
          <PromoBImage key={index} model={model} />
        ))}
      </div>
    );
  }

  return (
    <div className="g-mt-25 g-mb-50 align-center">
      <div className="promoB afn-shadow-dark">{promoBWrapper}</div>
    </div>
  );
}

export default PromoB;
