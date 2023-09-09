import { MenuItemModel } from "../models/MenuItemModel";

export const TopMenuDataSource: MenuItemModel[] = [
  {
    Text: "TV",
    Image: "",
    Url: "",
    Items: [
      {
        Text: "SCHEDULES",
        Image: "",
        Url: "http://v3.myafn.dodmedia.osd.mil",
        Items: [],
      },
      {
        Text: "SEARCH ALL",
        Image: "",
        Url: "/Search.aspx",
        Items: [],
      },
      {
        Text: "SEARCH SPORTS",
        Image: "",
        Url: "/ScheduleSports",
        Items: [],
      },
      {
        Text: "DTS SPREADSHEETS",
        Image: "",
        Url: "/ScheduleXls",
        Items: [],
      },
      {
        Text: "NEW \u0026 RETURNING",
        Image: "",
        Url: "/SeriesCycle",
        Items: [],
      },
      {
        Text: "DoD NEWS",
        Image: "",
        Url: "https://www.defense.gov/news",
        Items: [],
      },
    ],
  },
  {
    Text: "RADIO",
    Image: "",
    Url: "",
    Items: [
      {
        Text: "SCHEDULES",
        Image: "",
        Url: "/Radio",
        Items: [],
      },
      {
        Text: "AFN Go",
        Image: "",
        Url: "https://afngo.net",
        Items: [],
      },
      {
        Text: "DTS SCHEDULES",
        Image: "",
        Url: "/DTSRadio",
        Items: [],
      },
      {
        Text: "ARTIST SPECIALS",
        Image: "",
        Url: "/RadioArtistSpecials",
        Items: [],
      },
    ],
  },
  {
    Text: "COMMUNITY",
    Image: "",
    Url: "",
    Items: [
      {
        Text: "CONTACT US",
        Image: "",
        Url: "/Email",
        Items: [],
      },
      {
        Text: "FACEBOOK: AFN TV",
        Image: "",
        Url: "https://www.facebook.com/myAFN",
        Items: [],
      },
      {
        Text: "#THEBIGDEAL",
        Image: "",
        Url: "http://myafn.dodlive.mil",
        Items: [],
      },
      {
        Text: "MANAGE MY DECODER",
        Image: "",
        Url: "https://afnconnect.myafn.dodmedia.osd.mil/pw",
        Items: [],
      },
    ],
  },
  {
    Text: "FAQ",
    Image: "",
    Url: "/Faq",
    Items: [],
  },
  {
    Text: "CONTACT US",
    Image: "",
    Url: "/Email",
    Items: [],
  },
];
