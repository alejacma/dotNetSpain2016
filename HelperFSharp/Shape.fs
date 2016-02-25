namespace HelperFSharp

type Shape =
    | Circle of float // Radius
    | EquilateralTriangle of double // Side length
    | Square of double // Side length
    | Rectangle of double * double // Height and width
    member this.Area() =
        match this with
        | Circle radius -> 3.141592654 * radius * radius
        | EquilateralTriangle s -> (sqrt 3.0) / 4.0 * s * s
        | Square s -> s * s
        | Rectangle (h, w) -> h * w