import { MenuItemModel } from "../models/MenuItemModel";

export const LeftMenuDataSource: MenuItemModel[] = [
  {
    Text: "AFN TV",
    Image: "",
    Url: "",
    Items: [
      {
        Text: "SCHEDULES",
        Image: "",
        Url: "http://v3.myafn.dodmedia.osd.mil",
        Items: [],
      },
      { Text: "SEARCH ALL", Image: "", Url: "/Search", Items: [] },
      {
        Text: "SEARCH SPORTS",
        Image: "",
        Url: "/ScheduleSports",
        Items: [],
      },
      { Text: "DTS SCHEDULES", Image: "", Url: "/ScheduleXls", Items: [] },
      {
        Text: "#THEBIGDEAL TV BLOG",
        Image: "",
        Url: "http://myafn.dodlive.mil",
        Items: [],
      },
      {
        Text: "NEW \u0026 RETURNING",
        Image: "",
        Url: "/SeriesCycle",
        Items: [],
      },
    ],
  },
  {
    Text: "AFN RADIO",
    Image: "",
    Url: "",
    Items: [
      { Text: "SCHEDULES", Image: "", Url: "/Radio", Items: [] },
      { Text: "DTS SCHEDULES", Image: "", Url: "/DTSRadio", Items: [] },
      { Text: "AFN Go", Image: "", Url: "https://afngo.net", Items: [] },
    ],
  },
  {
    Text: "OTHER",
    Image: "",
    Url: "",
    Items: [
      {
        Text: "MANAGE MY DECODER",
        Image: "",
        Url: "https://afnconnect.myafn.dodmedia.osd.mil/pw",
        Items: [],
      },
      { Text: "CONTACT US", Image: "", Url: "/Email", Items: [] },
      {
        Text: "DoD NEWS",
        Image: "",
        Url: "https://www.defense.gov/news",
        Items: [],
      },
    ],
  },
];
