import { useContext } from "react";
import { AfnContext } from "../../modules/AfnContext";

import { Icon } from "../Icon";

export default function Links() {
  const { bottomMenuItems } = useContext(AfnContext);

  return (
    <>
      <div className="u-heading-v3-1 g-brd-white-opacity-0_3 g-mb-25">
        <h2 className="u-heading-v3__title h6 text-uppercase g-brd-white">
          Useful Pages
        </h2>
      </div>
      <nav className="text-uppercase1">
        <ul className="list-unstyled g-mt-minus-10 mb-0">
          {bottomMenuItems.map((item, index) => (
            <li
              className={
                "g-pos-rel" +
                (index < bottomMenuItems.length - 1
                  ? " g-brd-bottom g-brd-white-opacity-0_1 g-py-10"
                  : " g-brd-bottom g-py-10")
              }
              key={index}
            >
              <h4 className="h6 g-pr-20 mb-0">
                <a
                  className="g-color-white-opacity-0_8 g-color-white--hover"
                  href={item.Url}
                >
                  {item.Text}
                </a>
                <Icon iconName="ArrowRightShort" className="g-absolute-centered--y g-right-0" />
              </h4>
            </li>
          ))}
        </ul>
      </nav>
    </>
  );
}
