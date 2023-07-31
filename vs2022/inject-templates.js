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
	if (!process.argv.slice(2)[0]) {
		throw new Error('The entered parameter is empty');
	}
	const source = path.join(__dirname, './templates');
	const sourceTemplates = await fs.readdir(source);

	sourceTemplates
		.map((x) => path.join('templates', x))
		.forEach((sourceFolder) => {
			console.log(`Found template to move: ${sourceFolder}`);
			const archiveName = path.basename(sourceFolder) + '.zip';
			const outputZipPath = path.join(process.argv.slice(2)[0], archiveName);
			createZipArchive(sourceFolder, outputZipPath);
		});
})();
