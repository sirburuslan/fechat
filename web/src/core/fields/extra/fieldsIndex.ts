/*
 * @file Fields Index
 *
 * @package fc
 *
 * @author Ruslan Sirbu
 * @version 0.0.1
 * @updated 2024-03-15
 *
 * This file groups the extra fields for faster access
 */

// Import the fields
import FieldText from "@/core/fields/extra/collection/FieldText";
import FieldTextarea from "@/core/fields/extra/collection/FieldTextarea";
import FieldPassword from "@/core/fields/extra/collection/FieldPassword";
import FieldEmail from "@/core/fields/extra/collection/FieldEmail";
import FieldPhone from "@/core/fields/extra/collection/FieldPhone";
import FieldStaticList from "@/core/fields/extra/collection/FieldStaticList";
import FieldDynamicList from "@/core/fields/extra/collection/FieldDynamicList";
import FieldListManager from "@/core/fields/extra/collection/FieldListManager";
import FieldCheckbox from "@/core/fields/extra/collection/FieldCheckbox";

// Export the fields
export default {
    FieldText,
    FieldTextarea,
    FieldPassword,
    FieldEmail,
    FieldPhone,
    FieldStaticList,
    FieldDynamicList,
    FieldListManager,
    FieldCheckbox
};