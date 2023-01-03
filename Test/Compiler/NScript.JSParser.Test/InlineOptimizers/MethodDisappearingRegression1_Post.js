function baseless(i, j, k, l) {
    return i + j + k * l;
}

var add2 = function(i, j) {
    return baseless(i, 4, j, 12);
};

function regressionParent(win, laxy) {
    add2(win, function () {
        return laxy;
    });
}

regressionParent(1, 2);
