FROM node:22.12-alpine
WORKDIR /app

COPY ./src /app/src
COPY ./package.json /app/package.json
COPY ./package-lock.json /app/package-lock.json

ENV NODE_ENV=production


RUN npm ci --ignore-scripts --omit-dev

ENTRYPOINT ["npx", "tsx", "src/index.ts"]