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

// Example usage:
// const text = await extractTextFromDocx('path/to/your/file.docx');
// console.log(text);