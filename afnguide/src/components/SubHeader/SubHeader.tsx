import { Container } from "react-bootstrap";
import { Breadcrumbs } from "../Breadcrumbs";

function SubHeader(props: {
  title: string;
  subTitle?: string;
  crumbs?: string[];
}) {
  return (
    <section className="afn-bg-light">
      <Container className="g-z-index-1 g-py-30">
        <h1 className="g-font-weight-300 g-letter-spacing-1 g-mb-2">
          {props.title}
        </h1>
        <div className="lead g-font-weight-400 g-line-height-2 g-letter-spacing-0_5">
          <p className="mb-0 g-font-size-16">{props.subTitle}</p>
          {props.crumbs && <Breadcrumbs crumbs={props.crumbs} />}
        </div>
      </Container>
    </section>
  );
}

export default SubHeader;
