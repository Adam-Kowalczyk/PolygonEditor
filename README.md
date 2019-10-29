# PolygonEditor
GUI application to create and modify multiple polygons.

Possible operations:
- Add new Polygon: Press "Create new" and start placing new points on the bitmap. To end click the first point of the polygon.
- Add predefined polygon: Press "Predefined Polygon X"
- Move point: Click (left-click) and drag one point.
- Move edge: Click (left-click) and drag one edge.
- Move polygon: Click (left-click) and drag polygon.
- Delete point: Click (right-click) on the point and select "Delete point"
- Delete polygon: Click (right-click) on the polygon and select "Delete polygon"
- Add middle point: Click (right-click) on the edge and select "Add middle point"
- Add "Equal" relation: Click (right-click) on the edge and select "Add 'equal' relation". Then select second edge. Their length will be the same.
- Add "Parallel" relation: Click (right-click) on the edge and select "Add 'parallel' relation". Then select second edge. They will be parallel to each other.
- Delete relation: Click (right-click) on the edge with relation and select "Delete relation".
- Block/Unblock point: Click (right-click) on the point and select "Change blocked/unblocked".

![alt text](/example1.png)


Algorithm to maintain relations:
From modified line: 
if related line is invalid (doesn't keep relation):
  adjust modified line
else
  adjust related line

adjusting line:
  first choose end of this line that is not connected to other line with relation or that is not blocked
  change position of this point so that it keeps relation
  adjust other line that is connected to that point
