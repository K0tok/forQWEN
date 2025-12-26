const WordExtractor = require('word-extractor');

async function extractTextFromDocx(filePath) {
    try {
        const extractor = new WordExtractor();
        const extracted = await extractor.extract(filePath);
        let text = extracted.getBody();
        return text;
    } catch (error) {
        console.error('Error extracting text from DOCX:', error);
        throw error;
    }
}

// Test with our created file
async function testExtraction() {
    try {
        const text = await extractTextFromDocx('test_cyrillic.docx');
        console.log('Extracted text:');
        console.log(text);
        console.log('\n--- End of extracted text ---');
    } catch (error) {
        console.error('Error during extraction test:', error);
    }
}

testExtraction();