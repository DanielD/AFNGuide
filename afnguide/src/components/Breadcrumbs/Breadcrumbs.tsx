import { Breadcrumb } from "react-bootstrap";

function Breadcrumbs(props: {crumbs: string[]}) {
  const isLast = (index: number) => index === props.crumbs.length - 1;

  return (
    <Breadcrumb as={"ul"} className="u-list-inline g-font-size-16">
      {props.crumbs.map((crumb, index: number) => (
        <Breadcrumb.Item key={index} as={"li"} className="list-inline-item g-mr-7" active={isLast(index)}>
          <span>{crumb}</span>
        </Breadcrumb.Item>
      ))}
    </Breadcrumb>
  );
}

export default Breadcrumbs;
