/*
 * @file Index
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file groups the general fields for easier access
 */


// Import the fields
import FieldText from "@/core/fields/general/collection/FieldText";
import FieldTextarea from "@/core/fields/general/collection/FieldTextarea";
import FieldImage from "@/core/fields/general/collection/FieldImage";
import FieldCheckbox from "@/core/fields/general/collection/FieldCheckbox";
import FieldStaticList from "@/core/fields/general/collection/FieldStaticList";

// Prepare the fields
const Fields = {
    FieldText,
    FieldTextarea,
    FieldImage,
    FieldCheckbox,
    FieldStaticList
};

// Export the fields
export default Fields;