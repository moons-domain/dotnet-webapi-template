const { promises: fs } = require('fs');
const path = require('path');
const AdmZip = require('adm-zip');

function createZipArchive(inputFolder, outputFolder) {
	const zip = new AdmZip();
	zip.addLocalFolder(inputFolder);
	zip.writeZip(outputFolder);
	console.log(`Created ${outputFolder} successfully`);
}

(async function main() {
	const outputFolder = process.argv.slice(2)[0] || '.';
	const source = path.join(__dirname, './templates');
	const sourceTemplates = await fs.readdir(source);

	sourceTemplates
		.map((x) => path.join('templates', x))
		.forEach((sourceFolder) => {
			console.log(`Found template to move: ${sourceFolder}`);
			const archiveName = path.basename(sourceFolder) + '.zip';
			const outputZipPath = path.join(outputFolder, archiveName);
			createZipArchive(sourceFolder, outputZipPath);
		});
})();
