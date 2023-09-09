import { Icon } from "../Icon";

import { SocialMediaButtonModel } from "../../models/SocialMediaButtonModel";

export default function SocialMediaIcons() {
  const socialMediaButtons = [
    new SocialMediaButtonModel(
      1,
      "Facebook",
      "https://www.facebook.com/myAFN",
      "AFN TV on Facebook",
      "facebook"
    ),
    new SocialMediaButtonModel(
      2,
      "Twitter",
      "https://twitter.com/AFNtelevision",
      "AFN TV on Twitter",
      "twitter"
    ),
    new SocialMediaButtonModel(
      5,
      "Wordpress",
      "https://myafn.dodlive.mil",
      "TV Talk on Wordpress",
      "wordpress"
    ),
    new SocialMediaButtonModel(
      6,
      "Instagram",
      "https://www.instagram.com/afn.radio.television/",
      "AFN on Instagram",
      "instagram"
    ),
    new SocialMediaButtonModel(
      -1,
      "Phone",
      "https://myafn.dodlive.mil/AFN-Now/",
      "Stream AFN on your phone or tablet",
      "phone"
    ),
  ];

  function renderIcon(icon: string) {
    switch (icon) {
      case "facebook":
        return <Icon iconName="Facebook" />;
      case "twitter":
        return <Icon iconName="Twitter" />;
      case "wordpress":
        return <Icon iconName="Wordpress" />;
      case "instagram":
        return <Icon iconName="Instagram" />;
      case "phone":
        return <Icon iconName="Phone" />;
      default:
        return <></>;
    }
  }

  return (
    <ul className="list-inline text-center text-md-right mb-0">
      {socialMediaButtons.map((item, index) => (
        <li
          className="list-inline-item g-mx-10"
          data-toggle="tooltip"
          data-placement="top"
          title={item.Tooltip}
          key={index}
        >
          <a
            href={item.Url}
            className="g-color-white-opacity-0_5 g-color-white--hover"
          >
            {renderIcon(item.Icon)}
          </a>
        </li>
      ))}
    </ul>
  );
}
