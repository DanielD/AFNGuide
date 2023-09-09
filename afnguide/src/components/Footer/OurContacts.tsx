import { Icon } from "../Icon";

export default function OurContacts() {
  return (
    <>
      <div className="u-heading-v3-1 g-brd-white-opacity-0_3 g-mb-25">
        <h2 className="u-heading-v3__title h6 text-uppercase g-brd-white">
          Our Contacts
        </h2>
      </div>
      <address className="g-footer-map-bg g-bg-no-repeat g-font-size-12 mb-0">
        <div className="d-flex g-mb-25">
          <div className="g-mr-10">
            <span className="u-icon-v3 u-icon-size--xs g-bg-white-opacity-0_1 g-color-white-opacity-0_6">
              <Icon iconName="TelephoneFill" />
            </span>
          </div>
          <p className="mb-0">
            <a href="tel:19514132339" className="g-color-white-opacity-0_8">
              +1 951 413 2339
            </a>
            <br />
            DSN 312 348 1339
          </p>
        </div>
        <div className="d-flex g-mb-0">
          <div className="g-mr-10">
            <span className="u-icon-v3 u-icon-size--xs g-bg-white-opacity-0_1 g-color-white-opacity-0_6">
              <Icon iconName="GlobeAmericas" />
            </span>
          </div>
          <p className="mb-0">
            <a
              className="g-color-white-opacity-0_8 g-color-white--hover"
              href="mailto:afn@mail.mil"
            >
              afn@mail.mil
            </a>
            <br />
            <a
              className="g-color-white-opacity-0_8 g-color-white--hover"
              href="https://www.myafn.net"
            >
              www.myafn.net
            </a>
          </p>
        </div>
      </address>
    </>
  );
}
