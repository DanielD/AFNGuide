import { useContext } from "react";
import { AfnContext } from "../../../modules/AfnContext";

import "./VerticalMenu.css";

function VerticalMenu(props: {
  menuKey: string;
  afnTitle?: string;
  className?: string;
}) {
  const { leftMenuItems } = useContext(AfnContext);

  const filteredMenuItems = leftMenuItems.filter(
    (item) => item.Text === props.menuKey
  );

  return (
    <>
      <p className={props.className}>
        {props.afnTitle && (
          <h2 className="afn-title-color-light g-font-weight-600 g-font-size-40 text-uppercase g-line-height-1_2 g-letter-spacing-1_5 g-mb-20">
            AFN <span className="afn-title-color-dark">{props.afnTitle}</span>
          </h2>
        )}
        {!props.afnTitle && (
          <hr className="u-divider-db-solid afn-brd-color-dark g-my-30 u-divider-width-50" />
        )}
        {filteredMenuItems.length > 0 &&
          filteredMenuItems[0].Items.map((item, index) => (
            <div key={index}>
              <a
                href={item.Url}
                className="g-color-black g-color-primary--hover g-text-no-underline--hover"
              >
                {item.Text}
              </a>
            </div>
          ))}
      </p>
    </>
  );
}

export default VerticalMenu;
