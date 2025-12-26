// Симуляция проблемы с кодировкой и её решение

// Функция для симуляции искажения текста (для демонстрации проблемы)
function simulateEncodingIssue(text) {
    // Это искусственная симуляция искажения, которое может произойти при неправильной кодировке
    // В реальности это может быть результатом неправильной интерпретации байтов
    return text
        .replace(/о/g, '') // Заменяем 'о' на символ замены (может происходить при кодировке)
        .replace(/О/g, '') // Заменяем 'О' на символ замены
        // Добавим другие потенциальные искажения
        .replace(/л/g, 'л') // В реальном случае могут быть и другие искажения
        .replace(/г/g, 'г');
}

// Функция для восстановления текста
function fixEncodingIssues(text) {
    // Попытка восстановить наиболее часто встречающиеся искажения
    // Обратите внимание, что это эвристический подход и работает не во всех случаях
    
    // Восстановление часто встречающихся последовательностей
    text = text.replace(/злоговый/g, 'Залоговый');
    text = text.replace(/Оператор/g, 'Оператор'); // если 'О' искажается
    text = text.replace(/ОГРН/g, 'ОГРН');
    
    // В более общем случае, можно использовать библиотеки для определения кодировки
    // или более сложные алгоритмы восстановления
    
    return text;
}

// Оригинальный текст
const originalText = 'ЗалоговыйОператорОГРН';
console.log('Original text:', originalText);

// Симуляция искажения
const corruptedText = simulateEncodingIssue(originalText);
console.log('Simulated corrupted text:', corruptedText);

// Восстановление текста
const fixedText = fixEncodingIssues(corruptedText);
console.log('Fixed text:', fixedText);

// В реальных сценариях рекомендуется использовать правильные библиотеки и подходы
console.log('\nFor real DOCX files, use mammoth library:');
console.log('- It handles encoding better than basic text extraction methods');
console.log('- It works with the document\'s XML structure directly');
console.log('- It\'s less prone to character encoding issues');