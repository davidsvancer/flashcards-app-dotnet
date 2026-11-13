export function scrollLeft(element, amount) {
    console.log("scrollLeft called", element, amount);
    if (element) {
        element.scrollBy({
            left: -amount,
            behavior: "smooth"
        });
    } else {
        console.error("Element is null or undefined");
    }
}

export function scrollRight(element, amount) {
    if (element) {
        element.scrollBy({
            left: amount,
            behavior: "smooth"
        });
    }
}