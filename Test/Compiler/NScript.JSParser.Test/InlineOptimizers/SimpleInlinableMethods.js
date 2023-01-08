function add(i, j) {
    return i + j;
}

function isMatch(l, r) {
    return l === r;
}

function add2(i1, j) {
    return add(i1, j);
}

function add3(i, j, k) {
    return add2(add2(i, j), k);
}

add3(1, 2, 3);