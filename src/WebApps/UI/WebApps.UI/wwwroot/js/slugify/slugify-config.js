// Slugify function for both English and Vietnamese
function createSlug(value) {
    return slugify(value, {
        lower: true,
        locale: 'vi' // Supports Vietnamese language
    });
}