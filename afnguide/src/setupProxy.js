const { createProxyMiddleware } = require("http-proxy-middleware");

module.exports = function (app) {
  app.use(
    "/json",
    createProxyMiddleware({
      target: "https://myafn.dodmedia.osd.mil",
      changeOrigin: true,
    })
  );
  app.use(
    "/api",
    createProxyMiddleware({
      target: "http://localhost:5230",
      changeOrigin: true,
      secure: false,
    })
  );
};
