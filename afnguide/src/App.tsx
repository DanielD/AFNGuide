import { Routes, Route } from "react-router-dom";
import { Layout } from "./pages/Layout";
import { Home } from "./pages/Home";
import { Search } from "./pages/Search";
import AppContextProvider from "./utils/AppContextProvider";

import "bootstrap/dist/css/bootstrap.min.css";
import "./assets/css/style.css";
import "./App.css";

function App() {
  return (
    <AppContextProvider>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/search" element={<Search />} />
          <Route path="/" element={<Home />} />
        </Route>
      </Routes>
    </AppContextProvider>
  );
}

export default App;
