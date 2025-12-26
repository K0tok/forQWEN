const mammoth = require('mammoth');

async function extractTextFromDocxMammoth(filePath) {
    try {
        const result = await mammoth.extractRawText({path: filePath});
        const text = result.value; // The raw text
        return text;
    } catch (error) {
        console.error('Error extracting text from DOCX using mammoth:', error);
        throw error;
    }
}

// Test with our created file
async function testExtractionMammoth() {
    try {
        const text = await extractTextFromDocxMammoth('test_cyrillic.docx');
        console.log('Extracted text using mammoth:');
        console.log(text);
        console.log('\n--- End of extracted text ---');
    } catch (error) {
        console.error('Error during mammoth extraction test:', error);
    }
}

testExtractionMammoth();