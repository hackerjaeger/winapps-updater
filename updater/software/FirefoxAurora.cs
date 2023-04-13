﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "113.0b2";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "3f3d84abf434393108f221d9d90313c08f8db734e7626247d9e5c53f75643e3345b0cf47f3c9067b9f52b76162f62af0d00eead71d883762e1959eba948649c3" },
                { "af", "7357240df57fa5b8388c9786202a4503dc5f47d1d8029d6b4ad35846eb54a2b0ff0b01281ec53b1f72b9fd81bc9f2b96a6d4663ab868a65927eac85d1f9492e3" },
                { "an", "c67f3a99d657f446d6695803a581393fb9fdc22c64f5a194cfe20a08948afd2e62e99715b545cbb6df42978c70c7d2b6654e2c8671452918c23ae5f7bcc2e152" },
                { "ar", "880b3ed6a0392374be860f30199ae44ea49ca021d54b54801023ef1821d612d890a650acbb677d646a8a58e361f999a8cb9c2be635000b963e89ed44785a9f3b" },
                { "ast", "d6c3a466ca86a930c788743840ad5b1b90df7181aed9f8b31d6bca3ae41d2468e4cd88571f555e20c5b53df1b63d63e03d20ae71743500a92b058daaeb55df65" },
                { "az", "038f283dd8965fc8c8e3df043220d63681981429fcb8318fc3f22687f17cda2bb61eaaac5badfbd20870176776e7ed970deca9a44da839c53f52da6ab8d9192d" },
                { "be", "3aef2dff682c0ced2ad731a4890d98110da7e604781282928902381c925a670f05e867aaae374ad29b3cd2e0747b4a5d2fb23e4a71acc310029eac6992c306e1" },
                { "bg", "deb0762e927afa930f86ef8a23649b79bfdf8bbbf6f16cb9673e74aee18156d03fc31e65adebd647478d1cda2d078248e52df5d3565191ae37efd6ef965e0943" },
                { "bn", "09be3bb5ea211298c6f223fdbc5be2b6350af22b0bba3c0a88d245e2d7e717f61b8189ded36bd12ffc027152f70eff71f1e7fe4a4854e8e2420917fd8e4f9aa3" },
                { "br", "54b038810a26940b5b4e5425b765a0d75352e572149a6a970918715bf4d5364ed1eb3859d7f983cda4cf9896ef287d7bb974e6b8cbce66b18e7afc6773e57dcb" },
                { "bs", "e364a9a3bde1b96d8faebc10b2b2b49458da83abad1e9e4b4e0d51b859daf08749e1941e8a829655629a19c7580f80a66c9d22f7ce366ffc7875026ce0023817" },
                { "ca", "77b600eb49ea56312e92a8b471a11a481a2e09bd4578182783e8ad92a735b03c1ffd7528ac271bc5cfc766f98b245bfc0d61bced995e30f456a4dd0cf87eb8f0" },
                { "cak", "918a3df36a4fa1dce27eda41bd8420bba190b21fceef656c54b3825a79089d08c7e6055026197eff31f4930120bc0fb6d7e88bba974582871b6e9893fc170e24" },
                { "cs", "eeaf67ce1f776c1bf1c13d30236eba1f16d7479bc2145ce033f7769f7f2e11de03858cbf3c06ce443d9b129bfb70f6c14cdf9222ec7f565163202377a161a73d" },
                { "cy", "eb6fcb6da42f36692283bde2d35617d423a7ec70fb1a6fb1a4c80f7353b80e2d94986f3e50340da9d0d9b9e64ecbb5d45dda66f8fa81318594701402c0eba0a2" },
                { "da", "75b56e596f0f0f383ad34b1e037667b02f204fec74a445f9504642df57fc315f6840f624448f1af91e1332672fb7985751da6a829f9549b8ee61aa5c46970844" },
                { "de", "87d896b672973379c7a476d2d7051dea46fac51b3a296ab384f57bc66cf3b8021b25e1a11b19450419a5a470d404c92bb0b6130be094eee4991b76545afb53e7" },
                { "dsb", "13b3af09f7052bcf810dc6d10655240ad49d07c284dfd42d8576852817654488adb10fe2026a72d6f248feef8dec02c4da85db970baace89e0263816877c66a9" },
                { "el", "34e241ff944f25342cdc336933464a0fe35f5a78f0d27645f8483463ae7d2dcc2df3423e0fd4c2fee576a31e0d4bf633fbc7fd364b246ee8b624a4e9d1afee79" },
                { "en-CA", "18302e0749ca0bfa428e1eb398887e136ee33ac77d80e05254f1f59ebf4c72677a037762dff902b6321bbdb096084c89b2e911cae27e2c9f51f258d264841236" },
                { "en-GB", "0e7351397b418ff6e06c50f5f5204364a418864c368b8e408b232832599d7668736733c752b8ffcb2429913e7f5cf5e65ba05b2a35c46f881a8374a74de78ae7" },
                { "en-US", "0fffc6d7803b3c033c12e903014ab09c73a6848f5bd7168266d1adefdda562dddb40ff7035efaa9450bb55f49f08686dbb68c11443cbacd82b36665bb74ac50b" },
                { "eo", "cde08c2c6e080a6804a25400e2cad215ddaf961e4ebbd6c4513b4647642f0684dedbf462a2dca71fb3bc9557852bdcc77b1da9de72d1c183cbbac84c5ee37922" },
                { "es-AR", "6bf3a3e8b54e0601f51cf78b7bad53deb9f190032e1a9c439244aaa1e460b08fad380271ddf491022dab9ea7b1fc2bba4518e077e2256ccb71a8becddec35411" },
                { "es-CL", "2c27069906c4152686b74076f846a5d579dab0c2cd5a328e79b248ebb70d08b88738635ef3b25c794f46dea7c468c794e8bd17df7e2a293f4b4765343057579f" },
                { "es-ES", "378a9ef4666797ed43a3994553ffe8c9fd2dd52b51655ecbfc3079cad6f24de822e9f142613de329fb78478fb50cefd76b27b4e6ca9fd2cd963abd4a9e137f3f" },
                { "es-MX", "d968107a9a3b9115f461a11c2283e06c821eeb6234c1e1e912f2cf639afb99c2a14171dc5de744b815860ecefdb23d26327e5bfe747e830704b3f674545ab974" },
                { "et", "2f154bb70427143d0f713952a1b877a0b3d8a1979b582562a43502b5f25af9035e50cb13aaaf0a42bad89287e00bca97f5081b05e81cf3eea38b195b4993a735" },
                { "eu", "07c708d58ee25645aacd0b3cf63758f6af88ac3bdfb79fdc768368f35e32714c616acdaa6dfc514b73e3569a1f21cd84329c116dd14a9e5d0819a720a6f4777d" },
                { "fa", "73df0b764d5c3f12199cb1073b95ade7aec833207f2b1b3b37fb2db71f77c8a0d25aa4cd15c639223261e7c62a93756ba1e7ebdd3ce247a947b4f0e2ef961e3b" },
                { "ff", "625a8003b8eba57e4b05fc4304bc6c551c3a406215aca3c00a2e08ab1eb95249c600136dcc432fa0a85190076f9824598c88ce462031f35fffcd114f12f8514d" },
                { "fi", "faa6a59aa3d9e21339cde7c2a095bc9e4d0f4afc217348c6e9a0fb5ae25136bf4704747ac997f3bb073e9946187a8eabd593b46e57c1c744030f8218a6a7dfda" },
                { "fr", "eae3ef2afec8f668ca754621b29c8319a520d6884b6532dffa2985fcb204dc58e4cb87590bd567077b0ebd6331e28714fcee9df00d4b645672a254937c5a16bb" },
                { "fur", "1593bd3177d416e060974d83cce85e8b28e47eabec14a874f24bc1f6c0050175fe7339af2c1ce4009263c3e51e6039611da880e8d11042620e49770c1cd0624d" },
                { "fy-NL", "5e76450ffa61c478e609d4416248db91aedf74a1a54f3ed689153d81156f323e05dadc6b72d798de7607d81ac79bcfbbd972d3430a355722d674f7a60affd9d9" },
                { "ga-IE", "cb06b23657f39bae1b87137ae430b09e169110c7c62ac1d297b545ec70727c6cd3bbc9c8e50be3e0897ddf55ca3d742773886b205734ddb9b9edf5cb4fc1db0e" },
                { "gd", "6ed78c8d99cb5d934fdc90d500a04bdf497d3a498f05d4ed378b6eaf2830ffcb0209ef6bbf60ee7eca52477059aa59bc8e6a8624709f501413e1135cffc4a77f" },
                { "gl", "5f71c39129ae19e595074dddb4cbd644df616e770f97b00950f5a7e8f345da07d69142601096dbba6cddadd54c150471511c2e4c25f2ee7b5e8c37260d70e9fe" },
                { "gn", "abfd47702b8d55e45c6ff89fa6d5c2fb376bb218bc7e71f535218a767abe695bc80a6c27c4cd6f0d34be0266666a2b06184bd5954214f25fd86591882664f679" },
                { "gu-IN", "07c5bdb3bd49ac3618327cd50e5fef28e0d87391876d62b0bd00f3d843579124b12411bc06748ed51ef394cb77d13921408ee807a6c77f05572d7eb61a926d37" },
                { "he", "575d68d40005fee7b27216ffe126ab9baca840dc9f95e2bf0adcd2022bed4244fac956dde801e0683441de15007d457dc33deef5fb3f6d3c58b8470a1aa042ad" },
                { "hi-IN", "9bbb4f85b03e98edef3879100d9b26bbd49b7ff3d8ad91051160fe44b49eab0b916a0fba0cab47c721fb2e766cd4c1b795f468d71d8c568ada208e8b11cca7e1" },
                { "hr", "ad581ff2bb8eb2d455cae61e2a27643deb92394e0d02ff3d09af9b560e8420a1ad1fe6b2d0edadbbb3ff161bb405d063369e2ff720c96ea53224a3e12702190d" },
                { "hsb", "54f7003413deec84f0dfa5136ba15c630486a02a503b3c2d35f9b287639e090ae3d2bbbc10acc2dd4f98e22abc78539172e4f90c81c7d5e818efdbac5321b2d3" },
                { "hu", "792906c6825ea050222182adc9eb524c02dc582d9cb75ca4fab3f265992057223947e287bbeab69ec9146f198b60ae4bbcd5f85a85160e7c320100c7024b2790" },
                { "hy-AM", "88ef96beba9d88d22eeb82b41e989b90bfab581052627205584abdeb69ee6503f2b7587ef7960a649e64fb99dc765f6ef70df3351116a495b9dd9f52a62f5447" },
                { "ia", "24235e7de5f0e41c3eba374f6ff524b48cdd34c0f4d41edc0ed5847b8671583e07a55a103b62a926b29ea639c0e775edd5d29f23207c5d1f27349c1fc738fc6f" },
                { "id", "a26cc532bf9fe38ab009c1314bf8f706ba33e4c43ee0c532eb1082e798e52e56485811aa5d63e7293ee5a711783263da3e6c78231a71851fa5867017c9c19c4a" },
                { "is", "f33226b1a08de32de6e6216af72e3387f8227a22b4f6dae3428977848ed16c0d7c1ead31a6aea272ad4651a0fefb9d9aa890bb59da0bf68c05c5b720f5ee746d" },
                { "it", "b48d4157bcae4d26c7f3d0486891388d712cd7a61556f3a9130c0a15fb25795fc89b3042bd1d83e7979dab2ec3a677942a420fe7abfecc7d3b984de80d9e2f21" },
                { "ja", "4c09c0db9e150688376f1094dae61d10c4e9a4212b278acb691ef88dca0327f9ed911c5056fa47cc0a0489ac72a6df4802c90dd1f3eaba49dd15b9e2361709e8" },
                { "ka", "bd9e124dc4122bfc0582568a4698a651b5770b30e97a90bcd6da0375300b2aafa3ef81362c24feab06574cbab8fdc96087e8ae5d315f44be3290e8c2e59cf55d" },
                { "kab", "5e56d706c2559603046a1e69ec7a5741e7e112312f8632747cab1fbbd695e7ef5cff2a2bdafcbf26a480ff385812c74025563eaa617240baa83ff8c7ccc75a7b" },
                { "kk", "fb28dfd6ea8841b9f1e3b4ca12227d1b5c9c91bfe91c25f9fc4c48e3dcca8259ee553931060c2d7872de99c5d059e3648baf252d0fa6541057c28b7bdb7c6108" },
                { "km", "1a36032e33dde48f45ee63c04fcf071c47efed392be8b7696cf1d792e91b610f887181c7cb8fd079c46b5008ff9c127ca1653ebed1a55ae5a063ab292d58f44f" },
                { "kn", "07af668c0d9e5b4d10e8ffc8d721ae90103b05f929f9efbfccb96051f26540e87dfd41decd6279a81676ad7d451ad1096ae944cdf79fd2a931c39f78876d537c" },
                { "ko", "a7bffc85b65eedbd56fed8ceb545165b2679ba12014ae0afcf8c45e8a3ca90834387ef30913b4659a4a60a3b499452ae8f388fdbdf4dd2d83719da894d563794" },
                { "lij", "b0e3fc90398ce970eb6b0885f2eec83953345f5fdc2af87c7a9a07aa1ae3367230c7411bc165a898b92263777f256b871f1ac581ae22e6b83ea9cf2486740458" },
                { "lt", "287f1b9ef3c1cac25a4013544d80a8c5b44dca95c89ae95176a328d61e3e94930f1315e37dbcdc1b211329035d90478614c4c40876c9e605cbb5eb8aa55a8dd2" },
                { "lv", "065625b8ea9a557cdc95863f93b87a1af330be57e6f5660912ddccef66f3f88e07c52e5bb7834bfc129799fa932368ddd58ce29d827fc34f53661c49cee1e5ea" },
                { "mk", "77a59c54c84c5d7738626c6f12cc1feed2913e2f1a9a889bec0c88a61001877cbc562117e9ef6d53fdb6df0409d8a9ffdecfd6815b78bb93e9fa6e2ee23c9e3e" },
                { "mr", "3fef054474d479c06814a3a29340cdd88f4740170fa6368e13890d12e34f5911bcc61aac1b5935b56247aef14a888553053c1ae9c5d00dd25d4f301e3ed1460f" },
                { "ms", "ad53b4352b3e5c32d964f09f8b799b28fa85b63a492613ffd07fbe2d5d5b6cf1281c367442fd510c52f279147725e95bde3be7d62ba677df48edb1f4c2b8e02f" },
                { "my", "15a6a63d9bc7c55dc22b35dc84615cc08de712d5248122d8e961f0677a3a20df04cf80728921ff96ce3fb393a3cf3f19077d87a35071a2cf856dead484b619c9" },
                { "nb-NO", "7b48f7c9aa129755febff6b7af9a488a70faf64dfe423781009484950a6e1b62d26d190a44d4898e8eba4bc83810fecc121d426c18f3b3fb6c04b2ddf31e06cd" },
                { "ne-NP", "4ee82119812fe701939385d445000c0942508a813300af28ab723bbb64b710a19f2965696a84bf7e6b393aef3763b70ce102936a0703d5ed0ca402277b4aeb20" },
                { "nl", "78b64cb90cf64a56613afb882a5d4da1c16e8b712b9076e223d00cb308acc77903de9cd09d85432a5f090a0ce4f1fd3f96a96eca27995acb29b88470f9381c10" },
                { "nn-NO", "67821b3600f092f645af5d889a280205d4ffda3eb97a175dded0b5811959d2c6c8eeeacd220b0c4facf0bb91e3060b5d792e8f5213ab8209764d2c3918aaf873" },
                { "oc", "8fef59c0d6cd8ce20e34e486f6d3fce66081295b674b04ad577d655accf18f84a1be84484c0656542f249f3df0de55ec9e7e76d86f083aa342d377cd9082730b" },
                { "pa-IN", "0b47f4c9fe0b31846cc4c0e05a50b9a9094a92dad2938c24fd508aaecf86d7de37dee6cd0dc52bdd765830a0c8860166d14efdfbc9ab74bb5601105ff6383e0b" },
                { "pl", "3956dd7b6e6522e70e5decdc86f8eb1561a9125ec961f1cf07c8d2000945c58fe6400fa3b766e8516a9707f593b4721af0e22095d9f7326679763e9cd22dcf7b" },
                { "pt-BR", "d2ed1dda19d0a64b6015b64f0d5c22a8c5364411da2ed6b663715fa5a865c8b1cfaccd601c244e181b3ca7d154b172c142464ee9059b15f727e429adbcdaf526" },
                { "pt-PT", "2eeb743f3e2b79feaf745662359de2fba98c4047bbb6cada3759993a12579f2fa4ac7b8a0eeb3bee75079631178a10a7a35739093f823851913e3a1f9f9032fd" },
                { "rm", "ca909bf08bc16c7480619b239bd6c1ec3befdbf96e4b57abc510dc2d09613de0a72950458a5199574125b661bfe03a2c5430e5b0eb0939d821607f0e0be981f6" },
                { "ro", "9f0db48b792dbbc07c34a08236b8ebb0b86a44a3cf684f268ca89c57e411ef1823d753d13b4399590ac6f6ab022c049efc18550800633fd1d617aefe417eaab0" },
                { "ru", "f1be05907d007ca0a19533fd90067fd5b90c3f76f9be54a80ee5c7d7dd2df1fc7059a1b62f510b09e0498bb34dde50d79b3547849469bf4d54480c7643bb65ef" },
                { "sc", "bf02e62a4e56156d0a51ec17b8908f5761117182495ff60772bf2f1434c66bee54d217181f0e11e3f1c56ba2b8beb0eb55b123b3fdae63393395cbab0d32cbf3" },
                { "sco", "97ccd63fc5545c4f6497112053da620eff565385a61bbac7b2701c1628dd9b60c7f771288fd9ebb1c5950b5a35e3c5d733e418212245439aa0471c8ee9d84872" },
                { "si", "1fbe67f1f9d7292f52cac6ea5f89895468172b5a06c6557638df7c2887125a6e1f8e8ea83c87010f78cda1eb4bb69bc4e9d91f5b49d495e70ab380f14e9a1d95" },
                { "sk", "3a05cefd524f41e264e4ec95adafbcd1be245fda23d3289ff554b3c719e9f5f1bc8fc43ee582ba054bc669f4b52d353e5b128c23229236b334d35cc62a973818" },
                { "sl", "f370480aaa85029158fbd373a454eba6b52d2c6c816b3c675aa2e7c90578efc2c724116de480e4b7b140f82471ee182a8aa5732d0871f0e51f223a81ad94f0b1" },
                { "son", "0e24321f9800c9d69e24f198eb78ee3737292f7db629cd244ab4924c81c1ca01a0f8cade3440ca79c79ec935f12074bbea74d33f3a3ca218af7d9210a5e6f49b" },
                { "sq", "6f9355e838d03c2f650179f1413c42f33fc0d26c7b1636cea751de786d7c61b50c259da69860767e1c2a4bb6a821b8c33a6d6f320029f6889e03992599e1c389" },
                { "sr", "0031afea5726beb4db9d6f1f20ffae6bc320d9af28edebc574559c912a135ae590547c30b84903fa012d916c3386d68c24388e543b18f53887d96937c4c3ae79" },
                { "sv-SE", "da66f479bf7e38b4433fc1b2e664da35b45e0ea9c20bf5b1a9f6f04f6090c329dde5dabac3a962d58c633a7cc64f0e79698ec6788bad5cc4501e460297465b88" },
                { "szl", "7f7b1cb303dffe82ac20a2faa61fce627649a0881a44c07f79a664932d97b62e3a93649b4376dd1e1b44b96d5c1b1dbfe8f646c5e377010ae42b9a8d5b168cb8" },
                { "ta", "2fb3ff000e79be7e29df84a1977c9bf6050131cd4abf4c175384a38212a3293082807578a7b41dac0337baea7eff38544c4ecc249cfd7b1acd133ce08dc50b8a" },
                { "te", "bbba8ca53a8dfd0369f3c976f4cdf7ad4c20909bcd531a7f733db46de36524a2e10d353407f1ec357c0cfb82bfddceb6c5787279fc10e775574a66f7542cf506" },
                { "tg", "00405dda598a69c7345a98431a2d0007f08c9f6f59ab612c28f51a74270d690e1e9774a8c34f277b20a58a4131955b5ea244a40011280a24713753f35a0a40b5" },
                { "th", "8faaf7ebc6e288f5ccdf2fea60f608104caf491b19eaf4ec6eeb8a0a98f500675a035ba219b5d739967a5f55db91c8d0d490e52e1d6966fb75d5841b0717fd2f" },
                { "tl", "3ffeac367e080e7146d385cff4c75a201466969a319e7fb1bd83f14928f58fdd110e9057a4fdae138273a3e5127bd87bd02f7c5e377ff4c067938a0a386d156f" },
                { "tr", "cd7e229841d158f9d579371d39120c4e6199ab283a6084631bd6e04c37431897846af3f91591b594bb4f01e13b8cef129906447cf40209d6af6edc61d6861bae" },
                { "trs", "32371623cca13f0e8bc3c20a7b98b45bb6d5f2d0cfa02f8f45708c675cbe13d5cdabbfba8eaa9ad7c5b998ec44626148ee2707b6d45514b61fd1b8ccb5750cd6" },
                { "uk", "224e4f91792546369b06b60dac82680ef434d3dae16119408b7c170e8a09bf2d28e32c5f23be4ba962e3a135af1911af375ee50e69f85a5453e30503a4538598" },
                { "ur", "63c975c8fdadb6c20158789885e5a0172417ade22e4bc46b5c9f3b7327bb11c63a501b595cf0721e503f6789a3cabeb302d86b672ae1574a8d79346ee579e676" },
                { "uz", "f8b9127c2810057525424ae5dba7c73db08dce94fa9df73a28d73a43f98ac837f5db1e1659908e83171fd8c5d67007ee61bbdfce36be6479d2eff758ed36945e" },
                { "vi", "8ea99bdeb66120b992522c96f685b469e4f9987463dec5d7c17cb2b11eb86b6c8e96ebc5325e0f49d1b95b828cb99d737707ba133df09109153a989a847312a6" },
                { "xh", "ecfd007dccf8422aea26e64db0e852054286b5f4589728ad5a4f5b50c45e6e3b3abdd3181dd2276f6142384547a11ca687df7713400011f9a22045ae2dd82c7e" },
                { "zh-CN", "08b03c9c61f629d7cae583512e7ee9b9f5b0715b99fc006f42ef6a2a2d6198bd3dbb995f0fc429e67c45d414100f7ec14c099e29fed10c5143d68aa7c466a7a4" },
                { "zh-TW", "0fd2670054fce8e7690503c021ed699ca5b95b9acd1c4958db560907107acd2c6a4abed6c0e8600e4fcdd9dd28479e6da1e622a7d8f99eeacac900f9028e87ab" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "228b6cacad7da1fe190a9870b0ece4216c0e2c21d67905b5ef3105c03e4f75a9f7b4709a9bde474b0eac544f27072500f9e8d473b25ec2a04b46a037902b8a3a" },
                { "af", "82caf440438df6c18eba103a4e159984e1b961129d82df5394d566bf475873c070f4eb946e22399a554b15b9ebcf2fbe8ca495c43c4b29317806c4315e8c5acf" },
                { "an", "3e2839af380739e1209512ebd1d6eb63d29d205857836453f8afa95dd4b2c69dbe3ec3a48994e29fcede9654d17b46d5d9a830f3c8b997f39fdad089201633de" },
                { "ar", "30971f4d35d4cea844828c799e5f010b1cda5d61b8f26f924fcf6350f1829fb38cc65923f859bd3199a7d4ec7ee0681ecaa6a505ae13bb4a9c0aaf8b77e17f62" },
                { "ast", "53a364758f0ffecd1e5cc4e7874b09f0a2a2090f2f975db2248ff61641e9c990573d8e3fe297b34c728f309ea0bcee10bc3e56ee5d1f3c5e7c5aff22c03d404b" },
                { "az", "6a8b25f806b2aa342b538fe6864efa5e50f4e2a6659f8f8322a95dfa86f77c77bf501f6617b09d185f8f442956b2d6049f436bad8a53f915801d22498675622d" },
                { "be", "3a991e912bd1968f3eeb5d60b6e028fb2e04892b1f6bbda2512537edf34d4f1cba8f4092db61fe65d5525e1a17dba17f17fd65fc6cf43797e63ac1570bf59937" },
                { "bg", "a6c364a27dfd9e2df7384cf082a0c2dc3d51f7ce187cb0e3fc7560675579afcd76eb739d4a2dedbbdc4298427066194bda1e92c8f1364e437d28856d7430e837" },
                { "bn", "d77fe62e2011acab7c508d77801cdddefc405796deb99c8f76e4305645c575b8262968f6e9072b74981c0b788c4bb393a8f17bc44f211b84276e1196c1d45107" },
                { "br", "a5038ee9b1fa54b21159cb7d4a0bff3c2a1b02098103306a65c2801450d7aa3c1964bb4a7c9791c2b7c013140e2fd220e475e464d22088fa3788a8c3c2098294" },
                { "bs", "ae13826b94425a84029063e317b8d647f7fc630cca75db2ea2489a0c9fa11451912c2ca57537eeaf57a0eac25895c8cff07149cf71fb5920be455e7649f05624" },
                { "ca", "b51cf0554a31b782e8db17f28b30e50af6dbd36989b4180a8daa19544c51b7c4a8a27212b0e9d41cf1c1fdb17f057267f9a331e5a7d1fa0f9f5311c07a324585" },
                { "cak", "23065df91f9c8e3fa5d8c01fefb430ff46f55b365053c458759e201df0c30aa0208cb2fcf77724ce6a809ef04adc482a0181a477ae7675ae42c1e74aea7ce282" },
                { "cs", "eb2441b237626bdf3565ac3c302e8beec51ae61427461db1bd8c63074a583808257e00b39c04d8828bd5f0b2b04f878fc73fab4b99b2350d8de8980bbba321d2" },
                { "cy", "06c4dd9833f32e2de5de2e0721800ea04d6afa881b10cd8607381651609920efcbf64d48cfc96ca177ece4fb97b5cb470a665b4d4f2947cca0496a16829ed07a" },
                { "da", "0221e33122a3307dda5e03c0329cada56a96b5f848bea135107441bb48bd2fe17face377773696458410ccc467ce2df7e53d1b9c7c16ae95310ea15ec2e19dab" },
                { "de", "4437423ff43e87696697263668d5e1f96df199eb1d4793dd8161fdc6aa02b5a89e412a051252e838c7c742fda47455cfce5e6db709a25ec5f661b9982b987348" },
                { "dsb", "53873a390a7cb9b8d510c888d2ae060980d0528e62f9bec04f86961ac94c8889beb7aac01d54ae9535bbf0893218795746aad82dbbbb1c379857de22390132d7" },
                { "el", "77388424738283db5cb6be19876a8bdda6990fa3b9447512605a44b2e68a6e6cf2884514b20001b3602ea7ad30f3e2828d43109b01f731f09baa87fb9ab9d5ab" },
                { "en-CA", "9d8a0530d17d563b282f6b347607aca9725722ddd6f563e60917221e8b4999b90f12edca8f522b5809e6175a6f0f7f77320d25adbeea3e09d28632acb16dc0c6" },
                { "en-GB", "79cabf0ccde1697aa7ff7289870c8034fd3b056c0f91f3cf352a5f388ba688f67d421cefdb30935b50821e6ebe85c2c1d2d2bb3f2d6a3c02b59fe2dae403fe9f" },
                { "en-US", "efc0ad7319ca515277c7dc84e24a3d039e4b0906069114728cabb9ce807f012cb27f0c60d8b7100d16ec0e739bffd42a765026475d3a8ff2c60f0a352b0c0d20" },
                { "eo", "b7875ae120af24baeaa4d95eaf0323f0560d6f911457528d2046cd08c57b984665d7efd2c1e37b904d089dfccb9c8c2acff7358d1436aeb102590866081c43bb" },
                { "es-AR", "f89249a3485d186da760e83014512a027014bcd22ea330188db7670e4d9c46ef31acdc765e2c06775fbcb91316efacfab24f87e84da67a98e0cedfa0f9d5a1ea" },
                { "es-CL", "84fd1d10d348e9794007071fe7750429cb71583bc04e9c3a43c59ba6220606db92f9c5182cd56956dd77c66a5e685cf6d0ce55a3c855d63773018c67c862d9f1" },
                { "es-ES", "2904b8f6730dd3d3a0d2e29327187d544f6601e6261816a7b7ae8a5ddf377b955ae7d3431a514011117423d96b549323c91098a13143508a3d6d4c83e2dd79bb" },
                { "es-MX", "2f5c5473f19748329533ebab03d75b81509ffd47b6e3ace8589ab6c523b46a5ea40a89d5b5dde3f82e0e3ae40c9f5600a813ae1e28eb6516c74663e290a8801b" },
                { "et", "2cd4082e616c50c92105d646f211022e79464cf7ce928811d2b9c9fb5bf2021e3b39f987157919c61c305808b3af6edc71985ef4a011682fc8e364f98387d18a" },
                { "eu", "5b59f7a06b63656cb2f7ede0a0087b1476365d023ed0da44fdf9440180b8f0ceb9478439bc2295fe05f93d62b71504b6824df5d37bde395d8b44ac2b9f029de9" },
                { "fa", "a92d9b958fa13eb0b257b363b7591a44d05e3630c4fb7b194c91798f7c0eaf0247795777f23a3443011651ebbb7c63db437a3a2046d018ea8da05f4bbbec4f22" },
                { "ff", "c8fc6c287593b69790e8731d2cd1adfc0ebd914ce90106c199abed50f0bd11d86c97592c843cb83f49eddf2bf877d2375bc1dc5d8c7c053e5a0fbb2296c8471e" },
                { "fi", "1803a3762926357793b26ca3e732c2ac852167948a57f4ee1e232aaaf829ae0e08445081985b3b97037508abcd8e8e9775d0871617120232d3881a1b0a6c2408" },
                { "fr", "f39e5aef419c3042b4e7257191f8f207b3d0df4848b9313abcbe4a13d6f35de6d49caad5faf2a2edeeb0c4f5d22db2f0a7dd952345fccc1082e9562b75e86a8b" },
                { "fur", "695a78ff3b3aba0b11b11e4f0a55274c186f32be13216105c0b01ebbb7f9b0aec6f6df14363b788907f2f384b460bf20c68d8fd5c850d6ce2d41262cdac238c3" },
                { "fy-NL", "bb323ff5166022a24b0dd3eb9d0e9077369b3cacee26b6712b441fdf7f43421b9f7d61ec773810c83d6704e04b72a1fd41d9862d16f9ebf74e3aa54ac33ec765" },
                { "ga-IE", "a9267a7f6ff820ab2bbcf7ad568490603ae3077656810b81062868a552ea1f29344b35f14fd750b978d5b764ed06da6bb7acc49659e7f852ec647162da3b79ca" },
                { "gd", "f21ec825813f9943c22fdc5d21035244805c41954118f443ce647b0ee8763d67aec39cabddbacfaf68b77a5e051a72a9ad3606ed402ac39f359927decc9a97ee" },
                { "gl", "46160404b7a856c276f79f279fe84ff271fe31c48f1b4b27c43a489f8f171a4b76295520b28bf4c0222d9271f24af779c4e1e76909a5291f6e40ed47d2c08ee0" },
                { "gn", "d1f58d7a168f186900489c4d491220cf2683c41f48b574701ac579ba5a70127cdc70638ad93003fd134759e474cf53e9899d1473d783fc7282108b057c348cf8" },
                { "gu-IN", "e86c9aa8d8b2e35ebafd66c7dc24c98a4e6f9563a3843b4f56e6c850122c5781929c1288a52b39f3f7919edefc2ee027dfe3f37a8e62f0db5985f05e92155561" },
                { "he", "438e8f80f5ee9ffce253c55053a121e3fa68a527bfc9b7c30869747ae10ddd341afc63c9aa68f4e7cb03c6bd2223748d7a69c56528277f79a6ce44e110305c0d" },
                { "hi-IN", "8915488ab9922a475b9173ce2027f3a885dee81a4c3001cfc2ceb2530eabb8c30f21c3f475ea91fbd86794292b992a846cc676a116eaf508358a8c892a98e337" },
                { "hr", "08f453b063a13ca82536d5bf597f64ee424189872c9c4c78164a420916f3333e391872f8e63cfe6df3603b5c60856dd5e89a5ca9199accd7951c2426369ccae0" },
                { "hsb", "35e0435afd4fc7ba0e8c14f5a53d9e50cdda823b33137a3d6c4295b8c1a99c972e7b3b85433914ea1f45c0dae4f6a5638999bb7390e4bbc21fdb7b0ccb2a954f" },
                { "hu", "611479523d0b638353c5d5fc9d96ea56bc962589e83bf6fab94904fe3c8298ba928ae7a9fd422ac87728c2c6fce2df40b70a88fb70e95cdbc01d58a5bbf7c4b3" },
                { "hy-AM", "dadbb1d24ac1b2470e314e3a7a5b2fde58528a1082a242c3909fb0bfca37f336930c27d0a0bb70859224572265789987c7841f24e86436f508e903737dbefda7" },
                { "ia", "56b832e294cfa2e846b58f9c103ba88501ceace4be622efc54daac7fd27f0d850d1529a428b1626e9c6f647f5039475d4682b57fbbb62313990497913906a1ff" },
                { "id", "ef9e4a48865aa5a0763b9eec292c2c0c928a781ae5da7dc1f8385cc5822d3f66d34ec9fe6eb4a2f688187b2da01e5c4f86f8d9f07f655d327b7caab9235ad1d4" },
                { "is", "56c720e76ba80476f9ab355beca542c9101f8cc735ebd552351b397bc4e0af45f499e8c1e6268cbb27a554beb5a608ce8c3f59cfcdf4ed43d397424ca94eb67d" },
                { "it", "d9c7861e8223d75034dcfad2c2253a6f66dc8cd8beb8dd3467ea121fc7e2f1d2b5b16b0eccaac50f06a9db76041809b98257636c714ae9394d08f172fac77dbf" },
                { "ja", "57b7f63c35cd4c19b3e685405d99083edcd8bec77f4a4fda83489da55df449ec274828b52381e800f27c5980d62389aa17c56b9188edb51ace2ab1786e6e62c2" },
                { "ka", "6a2356699ce222be3e8dc33f904dde4c520918c710db5c19839f55340a3f27f2c7644d1a7dbb6a7e5f84be3cc8061bd25b67fbd2e65c5da2f939fd46fd0dd243" },
                { "kab", "f541db179d4b83b4ad038ab54727dc305551108962d06c12bdfd062e6a4bf25adb706951ae45afc22c676687cd8c52540cb4e42c84c247f90c8101ac0be1e423" },
                { "kk", "22b1dbd7e63ed5b636383be261b17e4f0c02b837a58224d31d94205b8f2133c9296311c54b2c5c53e4cda3992f86c1cba5a7b9eefe6ff10060cb4ad5a25650ea" },
                { "km", "a50a64ab4fef248e3187c68d772dd4a7d9d2d92bea8cae3f45bf585a5d8ba525250e858e91b6abd14f033401937977b13a7e1a5f56cf3c055959dbec30e2bcc0" },
                { "kn", "7b0c81d36930125409b4410e333a94444813b17a10ee17635720e4468df9383f206a9fc6dbeb292e36fdbc7066821433f8dc182cb65f627a96473344836b56a9" },
                { "ko", "bd9a778f9fcdfa90325aa4c6c25fd737f04a849f7af9152253f767f38fb56fb108d2773cf98dd6de438e8a6383cf532d3d0f1ddfb65c974c7d91fd567a9ea5d8" },
                { "lij", "89a5747a4b36e755619b95f7413e61f18fcee69d0f2407fcee3ebcf653711a8cdde51a839d3c6924631a5fccffcb5351167307054dd190a86ae10bcb82dc4cea" },
                { "lt", "2222e0b90e0ca9c1bb6e29696eaf3a802a379a09f3ed3d21481e54634cbb6ee2c1cf463ac9b5eda827eca67e7b051fd4875f6c3c836a72d40c5215dfce202664" },
                { "lv", "c9362665bc1a468107b1bc485abe927fe9b6922bf9b996271988e357750b0f8a18b849dab59259d9619cbaeee02e3e172255dc9dacaf6fc3302f85e202a933ce" },
                { "mk", "9d1687a19d27d04aa956891cf425d3b616585dca21187ef181cfeaba623bc2d9331a1c0ae3dd5e8c22fb5567ca3a5139a8f1dd8b632c92257451f6825b08a150" },
                { "mr", "8797016eb86f3e79f57ac0c53136d62b48e0da89bd9d51a2f8929c0229a77b111d27ba511d7cdff4ffc76b53206e1e3ccb0d84759fff1baef5c1fb2d73c01906" },
                { "ms", "650814c944fe51531c44e66ea67cf0ad304488a8436e7a7114cf7ed835d53053ca38ed7409a9773071b314b43a5f8c104360296feb12785d34883dbdfd365bde" },
                { "my", "bc4d96e6260fd1403f9f40d0c9d29ba01a27489f4946e1a3ebf09286e41403ebdd3f9767bd198a8a7a826da52798a69c3e79cb3925acf34afd4d3eed9ab2f905" },
                { "nb-NO", "73ed789014f9b0f163f162699124b46da8a9e40cbb123fa60f25085fd971372c65fb74e5ed99fa9998bc6a4e1451be368ed10dcb73c21ca410dc3079bc23beee" },
                { "ne-NP", "18a4d7acb0afca3515d032bd3e9a579dfe67240d6ec47a72103249291598337f72593e50c7cc7121d6f6d320127f733b4eae06c105433ae1cc0985b3b27522aa" },
                { "nl", "d43d466729167370483db2180120f1ff45373c6879b632613c2f1c7bac48a89e220423f9c1dfcfd2fcfe047b6801a8369532f495c4c1e05cd03c518834441c3a" },
                { "nn-NO", "ed29e2e36627f287a70d7bbd5d468e1f5d0e579ab5038ffb05ed39ca763706d07428be7f85295e65e39f9ddd46fcbbff343acd942bd03b95f493dda6a710fc8c" },
                { "oc", "01f38dd6b03ff7e8d35f2caa0391b4b047077b898b81673a601bbb2d1c4d4825c0942ec1a15d5f48b31ac6e0a2b1cb33161e95159524c074b0a8bd828bea333a" },
                { "pa-IN", "84ede6fa69d08d883d6194daa3209f4cc347cb354489280484f0bc2eb665bbfe66491e4af1b0d6a36a7d346c82f850c16453c35544bf1afc56ee8f358f412c7d" },
                { "pl", "3070fff70b46c7a33444b3b1c25c830abfd03be985a290b6223db0d400625aae353bbed461380cd093c7a329d524d53cdbee6d82f7a5269e1bfb3045db5fcc08" },
                { "pt-BR", "980c2890ad4fa171e6c7a535023ebf986775abfd6c5bf19910f7ae012c11c4147eae8f9831c666f44ece0f9779e118c6e4c90edc2b06b278d3d39ce4040d31eb" },
                { "pt-PT", "053321407854490f015e27c7387462604c3aca16a44ac68094627b4842ddfd103bf2fbe1b9ab4b55f3dea69b2994a17aeec545d03d0c567d9f800020f1db6af4" },
                { "rm", "41f423c44d0c54371111b213e001bef468b2ea0c8c1caee94ec60aff3179f0aa0ae8ed87fb63748d75e92d8e00992bdd4928bbb02af90aa2bf8a2c7f113418b0" },
                { "ro", "0b025dadfc2f1e8fd769c428e8f411e5e736932f27458bd100af8d94060887484c80677f6cd6bec625f7971990d159a00cf47d97337348dc671a0f8344b56a93" },
                { "ru", "b20be012bb56601bbc7511c27307ca00a9fde1d491697e2153fe877538c9fd08a1316acaada1321a4c3c20407ff597ac2620f5154fe6879cd79d8b193c6a08be" },
                { "sc", "70d9c14cd1485c987a98cdabb52baac8bb5718d65ef88529dc1996ac68bba922d8207738366b1cee47ed72464002cdc408a89b672415846c7f5b216a119f6129" },
                { "sco", "58a5f66f6d45c53da1274ae420dac101399f87091720646e76b843497ad845971a87118ae9084a6d8baed49ef02056ec1c4a7c6f3b6de5b9d5d2b82f7f4fd018" },
                { "si", "1fa0a43fec43b6eeac6913f13ab19f0fb0b3ffff31a1b1f496ac69713055a6781296a383f4c891b30b0ea232891c13d4929cfae6e267556338c803da9aff5b67" },
                { "sk", "e516222c5bcc8e66f958fc91f5b6af1d910b4c35bda8234b0c4f3e54b4856ddb6cdd4a45fec48d3a97f90ea823e51905313342fb37e09b0ef1b57fa750332436" },
                { "sl", "78775a4674151c6959ff3f9b72afbffb98216341ac9fd74ee284614e2a18da849e733a684951e8c26543dd6336bb8e826649b487fd61dafbe1133c0bb79ca705" },
                { "son", "03a28ae242d001dc439b799e84ebbf05d2043ab1b84f9a38adb8c809898de3262ab08913c59e8924f2bb5b30d405917363e1d5ee2adc326ed0ea891aa6088184" },
                { "sq", "fe40b435eb69c41c25d1723ad16d4b5c5862b642334840cb5f40cdd8a3ae6682734349d4df1ca551fae58bcb3c11edbbb5e852da028df931a203c4c5240c2fac" },
                { "sr", "c7ccb58ff63d23bf5b5883780ed60b2d21468799b1ddfa81d37735ac6f9eda7f599d15e317fc0c363713c8b365ddd73a9116c7d72c43cfce041889d08aa29910" },
                { "sv-SE", "4024a66f1e736f57b21da1fd07755ded28d2ced00b58d752280060719598dace18cc048b2715ee6eb1107a2668b2c68667895d78f4094af42e3cdf789c2d18ca" },
                { "szl", "f507da57c1327298412633109b7ce8fa017cdc3c60b67c80a66f5b896291bb6621ff210dacfd318836d8fa43548940942d4d55abf2764e57987a7093912613a8" },
                { "ta", "bdbf7c51a6c6e602bc73aa17827993728d4cbdc49a4835ca259a32b95d4ba204fe0d0b4643196c867092e1ea8b17bd58d35ad875434b4597d55b3a5f98fbf37d" },
                { "te", "a65027eb77c722fe676df618147e07b95edf1caba02121548c304a1b2e65d3ede7620ec59d16dcca5663dad8f2c38027c722db7dbc762411737c922979ea4b5d" },
                { "tg", "f4694790d2e8af423aa996a39b8412f8095e4ccd965f4a6bffb30187788a4fd0d67a52370ebe9b107ddfca37a15954cf65f1ed897432b3111441d250cd5a9af1" },
                { "th", "d89b6d0debc9f65151e4d259c66cce95d915a35d2504c1be865dbe0bbfc010574b1611368e3787f82640341ebbf4e7a29570866d604daf585ad01a31d6d761b8" },
                { "tl", "9db1639e9ff11f900306591270042f280daaa3a61460dc2c99e79623eb1380ee10bde3fd4dcc759c55cc4c7575cffb7d0374551c4b08f072b5f2acd9eace5b39" },
                { "tr", "e9f387a6b37359c5767d1b8ca13c7718004c39dc755e6a37fcf66f579e6caa5b75b1416fa22bf5f240e8bda1f33f118a3a82a3993cc11d7398c66deb4aca1049" },
                { "trs", "da6d57ab80285d4cb5f94ef7cca7a18e6239af7550649e1c996ad0ae564ee43c7c3e1bc5cd25df7539c58492938e9f239435aaaf5c25937c3d5a84c4f14d675d" },
                { "uk", "1f382173ef4f7f70d682ffe94f1bdf590f7188b3739af4039bb9830065d13d9e37950e13a42f97e0a2c315b829f083eddf8b1c500bf52db2cd7ecce2cdb38a05" },
                { "ur", "70620afe3c08a78994cc3c3f6c1fe36fede68a8ec03fed9fbfd822c30c07f611f672024a4710da88ea46731acfb22050bd38a5fa5f121eb108cf09a482d6b279" },
                { "uz", "dee7493a177f8d98ec62042508614c3bc5255caf7fe9e372ec18247a0c3019aafec627055448ddf55a2ef960ff14d080e970f4bae82a642c28dbfd32c9fb743c" },
                { "vi", "584bb88b17c73a772a10d52ff4958dded2f84cb1dbe737654e57b566b34f880b0d92cf826376ab503a88df70366aa3d4d2936323441b5cdd078d28a0024add9a" },
                { "xh", "f5789cf1939d8e920691bb4eaeecb414d1f5deeddd2b174a20109051d2bc549d7c675082ca6f893579144e12f112264d01a17dfd1dfaf2c5c61314801b2d8ce3" },
                { "zh-CN", "515c4a57632137573ac763b2c5be49a61363ef2c311674f136f9b04910847aac238c3a1a82ce3c585ffa5f0e26a5efe5c26dc539119f5f93108f702cc98bd51e" },
                { "zh-TW", "a6c4c74f44ab258e80a3bae9091273b77e19923d120a62544675973d1c671ea6df585a39aab0ba60644c31b25b4f1273e772356e8f8381559feaacca421cce65" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
