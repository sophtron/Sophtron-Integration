FROM node:17

WORKDIR /app

COPY ./application ./

RUN npm install 

ENV Env prod

EXPOSE 8080
CMD node ./index.js