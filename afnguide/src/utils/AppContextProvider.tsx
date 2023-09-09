import * as React from "react";
import { BrowserRouter } from "react-router-dom";
import { CookiesProvider } from "react-cookie";
import { AfnContextProvider } from "../modules/AfnContext";

const AppContextProvider: React.FC<{ children?: React.ReactNode }> = ({
  children,
}) => {
  return (
    <CookiesProvider>
      <BrowserRouter>
        <AfnContextProvider>{children}</AfnContextProvider>
      </BrowserRouter>
    </CookiesProvider>
  );
};

export default AppContextProvider;
