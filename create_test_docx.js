const fs = require('fs');
const { Document, Paragraph, TextRun, Packer } = require('docx');

// Create a new document with Cyrillic text
const doc = new Document({
    sections: [{
        properties: {},
        children: [
            new Paragraph({
                children: [
                    new TextRun('ЗалоговыйОператорОГРН'),
                    new TextRun({ break: 1 }),
                    new TextRun('Это тестовый документ с кириллическим текстом'),
                    new TextRun({ break: 1 }),
                    new TextRun('Проблема с кодировкой может возникнуть при извлечении текста'),
                ],
            }),
        ],
    }],
});

// Write the document to a buffer and then to a file
Packer.toBuffer(doc).then((buffer) => {
    fs.writeFileSync('test_cyrillic.docx', buffer);
    console.log('Test DOCX file with Cyrillic text created successfully!');
});