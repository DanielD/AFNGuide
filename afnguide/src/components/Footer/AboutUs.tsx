import afn_logo from "../../assets/images/afn_logo.png";

export default function AboutUs() {
  return (
    <>
      <a className="d-block g-mb-25" href="/">
        <img
          src={afn_logo}
          className="img-fluid"
          alt="AFN Logo"
          style={{ height: 33 }}
        />
      </a>
      <p className="g-mb-0">
        We are the American Forces Network, AFN, while you may know us by one or
        more of our many names, for more than 80 years we've delivered the best
        in news, information, and entertainment programming to American forces
        serving overseas.
      </p>
    </>
  );
}
