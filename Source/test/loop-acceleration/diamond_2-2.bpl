procedure main() {

  var x: int;
    var y: int;
 x := 0;
 assume (y >= 0);

  while (x < 99) {

    if (y mod 2 == 0) {

      x := x + 2;
    }
    else {
x := x + 1;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x - 2;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x + 2;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x - 2;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x + 2;
}

    if (y mod 2 == 0){
 x := x + 2;
}
    else {
x := x - 4;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
      x := x + 4;
    }

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x + 2;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x - 4;
}

    if (y mod 2 == 0) {
x := x + 2;
}
    else {
x := x - 4;
}
  }

  assert((x mod 2) == (y mod 2));
}
