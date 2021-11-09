'use strict';

const axios = require('axios');
const cheerio = require('cheerio');
const fs = require('fs');

const API_URL = 'https://do-ams3-17.hw.webhare.net/services/wobcovid19-prod-1/search/?count=100&orderby=publicationdate';
const BASE_URL = 'https://wobcovid19.rijksoverheid.nl';

GetDocumentList(API_URL);


class WOB {
  constructor(document) {
    // Substring is taken to remove the # from the id.
    this.Id = document['link'].substring(1);
    this.Url = `${BASE_URL}/publicaties/${this.Id}/`;

    this.Title = document['publication']['title'];
    this.Summary = document['publication']['summary'];

    // Substring is taken to only retrieve yyyymmdd from date.
    this.Date = document['publication']['publicationdate'].substring(0, 10);

    this.FileName = `${this.Date}_${this.Id}.json`;
  }

  parse(data) {
    // Parses HTML string, finds and returns linked WOB document info.

    const html = cheerio.load(data);
    let documents = [];

    html(".publication__download").each((_, publication) => {
      documents.push(this.parseDocument(html, publication))
    });

    this.Documents = documents;
  }
  
  parseDocument(html, publication) {
    // Retrieves meta data from string...

    let href = html(publication).attr('href')
    let name = html(publication).find('span').text();

    let metaElements = html(publication).find('.meta__line');

    var [fileType, pages, size] = metaElements.first().text().split("|");
    var [documentType, date] = metaElements.last().text().split("|");

    let document = {
      Name: name,
      Href: href,
      Url: `${this.Url}${href}`,

      FileType: fileType.trim(),
      Pages: pages.replace("pagina", "").replace("'s", "").trim(),
      Size: size.trim(),
      DocumentType: documentType.trim(),
      Date: date.trim()
    };

    return document;
  }
}

async function GetDocumentData(document) {
  // Retrieves HTML from the given URL and formats into document objects.
  let wob = new WOB(document);
  let result = await axios.get(wob.Url);

  wob.parse(result.data);
  saveWOBFile(wob);

  console.log(wob);
}


async function GetDocumentList(url) {
  // Retrieves HTML from the given URL to WOB document list.
  try {
    let res = await axios.get(url);
    console.log(`http status: ${res.status}`);

    res.data.results.forEach(async document => {
      await GetDocumentData(document);
    });
  } catch (e) {
    console.log(e);
  }
}


function saveWOBFile(wob) {
  var path = require('path');
  var location = `..\\WOB\\${wob.FileName}`;

  var json = JSON.stringify(wob);

  fs.writeFile(path.join(__dirname, location), json, function (err) {
    if (err) {
      console.log(err);
    }
  });
}
