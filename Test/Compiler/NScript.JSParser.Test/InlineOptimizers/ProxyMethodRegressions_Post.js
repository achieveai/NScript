function add(i, j) {
    return i + j;
}

function add2(i1, j) {
    return add(j, i1);
}

function add4(i3, j, fn) {
    return fn();
}

function add6(i4, j) {
    add(i4, j);
    add2(i4, j);
    add(i4, j, 3);
    add4(i4, j, add);
    add(i4, j);
}

add6(1, 2);
