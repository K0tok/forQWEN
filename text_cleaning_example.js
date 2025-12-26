const mammoth = require('mammoth');

// Функция для очистки извлеченного текста
function cleanExtractedText(text) {
    // Заменить потенциально искаженные символы
    // Убирает специальные символы, которые могут появиться при неправильной кодировке
    return text
        .replace(/\uFFFD/g, '') // Заменить символы замены (replacement character)
        .replace(/[^\x00-\x7F\u0400-\u04FF\u2000-\u206F\u2E00-\u2E7F\u3000-\u303F\s]/g, (match) => {
            // Здесь можно добавить логику для обработки конкретных искаженных символов
            console.log(`Found potentially corrupted character: ${match}`);
            return match;
        })
        .replace(/\s+/g, ' ') // Заменить множественные пробелы на одиночные
        .trim();
}

async function extractAndCleanText(filePath) {
    try {
        const result = await mammoth.extractRawText({path: filePath});
        let text = result.value;
        
        console.log('Raw extracted text:');
        console.log(text);
        console.log('\n---\n');
        
        // Очистка текста
        const cleanedText = cleanExtractedText(text);
        
        console.log('Cleaned extracted text:');
        console.log(cleanedText);
        
        return cleanedText;
    } catch (error) {
        console.error('Error extracting text from DOCX:', error);
        throw error;
    }
}

// Тест с нашим файлом
extractAndCleanText('test_cyrillic.docx')
    .then(result => console.log('\nExtraction completed successfully'))
    .catch(error => console.error('Extraction failed:', error));