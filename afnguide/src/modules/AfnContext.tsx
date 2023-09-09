import { createContext, useState, useCallback, useEffect } from "react";
import { useCookies } from "react-cookie";

import { BulletinModel } from "../models/BulletinModel";
import { SocialMediaButtonModel } from "../models/SocialMediaButtonModel";
import { MenuItemModel } from "../models/MenuItemModel";
import { PromoModel, PromoScheduleModel } from "../models/PromoModel";
import { TimeZoneModel } from "../models/TimeZoneModel";
import { ChannelModel, ChannelTimeZoneModel } from "../models/ChannelModel";

import { TopMenuDataSource } from "../data/TopMenuDataSource";
import { BottomMenuDataSource } from "../data/BottomMenuDataSource";
import { LeftMenuDataSource } from "../data/LeftMenuDataSource";

import facebookIcon from "../assets/images/buttons/facebook.png";
import instagramIcon from "../assets/images/buttons/instagram.png";
import phoneIcon from "../assets/images/buttons/phone.png";
import twitterIcon from "../assets/images/buttons/twitter.png";
import wordpressIcon from "../assets/images/buttons/wordpress.png";

export interface AfnContextProperties {
  bulletins: Array<BulletinModel>;
  socialMediaButtons: Array<SocialMediaButtonModel>;
  topMenuItems: Array<MenuItemModel>;
  bottomMenuItems: Array<MenuItemModel>;
  leftMenuItems: Array<MenuItemModel>;
  promoAItems: Array<PromoModel>;
  promoBItems: Array<PromoModel>;
  downloadPromoItems(): void;
  timeZones: Array<TimeZoneModel>;
  channels: Array<ChannelModel>;
}

export const AfnContext = createContext<AfnContextProperties>(null!);

export const AfnContextProvider = (props: { children: any }) => {
  const [cookies] = useCookies(["timezone"]);
  const [bulletins, setBulletins] = useState(Array<BulletinModel>());
  const [socialMediaButtons, setSocialMediaButtons] = useState(
    Array<SocialMediaButtonModel>()
  );
  const [topMenuItems] = useState(TopMenuDataSource);
  const [bottomMenuItems] = useState(BottomMenuDataSource);
  const [leftMenuItems] = useState(LeftMenuDataSource);
  const [promoAItems, setPromoAItems] = useState(Array<PromoModel>());
  const [promoBItems, setPromoBItems] = useState(Array<PromoModel>());
  const [timeZones, setTimeZones] = useState(Array<TimeZoneModel>());
  const [channels, setChannels] = useState(Array<ChannelModel>());

  const downloadChannels = useCallback(() => {
    fetch("/api/Channels", {})
      .then(async (response) => {
        const data = await response.json();
        const transformedJson = data.map((item: any) => {
          const channelTimeZones: ChannelTimeZoneModel[] = [];
          item.channelTimeZones?.forEach((channelTimeZone: any) => {
            channelTimeZones.push(
              new ChannelTimeZoneModel(
                channelTimeZone.timeZoneId,
                channelTimeZone.startTime,
                channelTimeZone.endTime
              )
            );
          });
          return new ChannelModel(
            item.id,
            item.title,
            item.channelNumber,
            item.color,
            item.isSplit,
            item.isSports,
            channelTimeZones
          );
        });
        return transformedJson;
      })
      .then((data) => setChannels(data))
      .catch((error) => console.log(error));
  }, []);

  const downloadPromoItems = useCallback(() => {
    const timeZoneId = cookies.timezone ? parseInt(cookies.timezone) : 1;
    fetch(`/api/Promos/${timeZoneId}`, {})
      .then(async (response) => {
        const data = await response.json();
        const transformedJson = data.map(
          (item: any) =>
            new PromoModel(
              item.title,
              item.description,
              item.imageUrl,
              item.linkUrl,
              item.isPromoB,
              item.schedules.map(
                (schedule: any) =>
                  new PromoScheduleModel(
                    schedule.channel,
                    schedule.date,
                    schedule.time,
                    schedule.timeZoneId
                  )
              )
            )
        );
        return transformedJson;
      })
      .then((data: PromoModel[]) => {
        const promoA = data.filter((item) => !item.IsPromoB);
        const promoB = data.filter((item) => item.IsPromoB);
        setPromoAItems(promoA);
        setPromoBItems(promoB);
      })
      .catch((error) => console.log(error));
  }, [cookies]);

  const downloadTimeZones = useCallback(() => {
    fetch("/api/TimeZones", {})
      .then(async (response) => {
        const data = await response.json();
        const transformedJson = data.map(
          (item: any) =>
            new TimeZoneModel(item.id, item.name, item.abbreviation)
        );
        return transformedJson;
      })
      .then((data) => setTimeZones(data))
      .catch((error) => console.log(error));
  }, []);

  const downloadBulletins = useCallback(() => {
    fetch("/api/Bulletins", {})
      .then(async (response) => {
        const data = await response.json();
        const transformedJson = data.map(
          (item: any) => new BulletinModel(item.id, item.title, item.content)
        );
        return transformedJson;
      })
      .then((data) => setBulletins(data))
      .catch((error) => console.log(error));
  }, []);

  const downloadSocialMediaButtons = useCallback(() => {
    fetch("/json/TVButtons.json?_", {})
      .then(async (response) => {
        let data = await response.text();
        data = data.replace("$afn.ProcessSocialMediaButtons(", "");
        data = data.replace(")", "");
        const json = JSON.parse(data);

        const transformedJson = json.map((item: any) => {
          let icon = "";
          switch (item.Title as string) {
            case "Facebook":
              icon = facebookIcon;
              break;
            case "Instagram":
              icon = instagramIcon;
              break;
            case "Phone":
              icon = phoneIcon;
              break;
            case "Twitter":
              icon = twitterIcon;
              break;
            case "Wordpress":
              icon = wordpressIcon;
              break;
          }
          return new SocialMediaButtonModel(
            item.Id,
            item.Title,
            item.Url,
            item.Tooltip,
            icon
          );
        });
        return transformedJson;
      })
      .then((data) => setSocialMediaButtons(data))
      .catch((error) => console.log(error));
  }, []);

  useEffect(() => downloadTimeZones(), [downloadTimeZones]);
  useEffect(() => downloadBulletins(), [downloadBulletins]);
  useEffect(() => downloadSocialMediaButtons(), [downloadSocialMediaButtons]);
  useEffect(() => downloadPromoItems(), [downloadPromoItems]);
  useEffect(() => downloadChannels(), [downloadChannels]);

  return (
    <AfnContext.Provider
      value={{
        bulletins,
        socialMediaButtons,
        topMenuItems,
        bottomMenuItems,
        leftMenuItems,
        promoAItems,
        promoBItems,
        downloadPromoItems,
        timeZones,
        channels,
      }}
    >
      {props.children}
    </AfnContext.Provider>
  );
};
