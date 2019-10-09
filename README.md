### CECS-529-Search-Engine-Project
 A semester long project implementing search engine
# Habeas

## Milestone1
- [Milestone 1 Requirements](docs/milestone1-summary.md)
- Demo: Oct 8

### Supported features
#### Search Queries
- Single query `ants`
- Phrase query `"dancing ants"`
- Or query `dance + ants`
- And query `dance ants`
- Near query `[dance near/2 ants]`
- Wildcard query `danc*`
- Author query `:author name`
#### Special Queries
- Index another corpus
- Stem a term

### Design
- Index: Positional-Inverted-Index
- Token Processor: 1) lowercase, 2) removes `-`, `'`, `"`, non-alphanumerics, 3) stem (e.g. dancing, dances -> dance)

### GUI
- used Electron.Net
- screenshots
![v1.3_start](docs/screenshots/v1.3_shot1_start.png)
![v1.3_search](docs/screenshots/v1.3_shot6_search.png)
![v1.3_content](docs/screenshots/v1.3_shot7_content.png)



