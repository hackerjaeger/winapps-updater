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
        private const string currentVersion = "112.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b5/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "5dbab327f4439df34e45dc4d51914c578f8ffde990fb3393abdcce20731523eb8c82c02ca5f2fe1d3dbd992544c9ac26a47167be500a9fd50bd259230883357e" },
                { "af", "bc293abaecdb58d1371533134a777d5c9826a301a31e196fab4028166116cc27c9a4d63d629d711f37ca58e31b76e0f80a90de1a1d382058ddb8ba3383d9da44" },
                { "an", "21623b4082edf5c31ef35e83fb4e295806a15080177d2a72efab43eb3b7a05116cdece3968b01ca59664a258b4adfd8e12d4cc6893821b9cc70b56539ac4382b" },
                { "ar", "001c9bad0e13300a48056104a21b87b7157940fc2e362519f8543031ec787819188d1422c144eddf5ac72f58508cbdd0e7a0eccc92f3d6025b47254f8280a035" },
                { "ast", "27a85062521d8d96c251ae4c7b56566130a6b46f0f99908839d6d4cb825e8cfac098c8877b167f5db6cb257bcf6241565ffda6983c6efd79bdee75becaf3bca8" },
                { "az", "a4b4cd811ba28901b42cde916274ecf8f573aca05a93106cf3ba50b4b349b4c95ad5be6e320aa8b0b5ed581db82a84ce5ea4c0a604678071e65bf8f2a9401c4c" },
                { "be", "c6c34333ea000165cee71e2cf4c1317b9bbdd22d5513ff9fae10faa6e256539b9bb814c7b1aab1c36d4930eef4580dac8100c742002f752024475ddeaf773038" },
                { "bg", "3afb4b3afa8af77b3db5f7c17fb1804084224e71ca4be1419ec5e5f44b539cefc854dd0b8a82326be8905df0cee0460c672c69fc587fd866d0380974088a4747" },
                { "bn", "f1a426f91bcde858d406481fa95eeeb2d401697ae9f71e2d4db3e97a4be411a77167900c66cb9a916253b186430325af2aaab8d9c12153b455c727328ccae6b0" },
                { "br", "95f2cc487ee2861da140699d3c7241a511cd904203b6d9d9320b2b11aaa5e576c947c5b1ffad8c2db6cd3f2b8c8288f989c1fc9148f3b431743916b68e50d595" },
                { "bs", "40931cc77bf989dbd93b051a8ea836aa56f89fe93c853cb958141eabcd3454966c4c1b0bce69ad358d361e20ebf48afe1f05cdf3d7f97a2fdba24545c6681171" },
                { "ca", "6b80cb995dd133e1ce12338cacb2b2938c4ef60fe373394df96ae49382049af33934596024d3a6f50dd099b3ffc515b68a29d7e26914a2ea0811b867a1bcc709" },
                { "cak", "c95a54d9e5e95761fba6ce59e57e26dc9103e3eb36dc404bd3813461ebcbf0aa937ed15fa4aa74a78395f5133721d18c120d0eb0b2605e088024389a1829af80" },
                { "cs", "02810bdc2eb3d89f51c278ecc87fff0fd8d8b403ab7a492ba597fad084ab997a9671568a1ef5301b09ef116c86850e4aba15de1ce13f67b7fc41de166d325396" },
                { "cy", "553601690ed9960bb785ea7b1478e30b0bbe02ea313143fd144f3500065d725344e990bb883117571555d985aeab5bfde5fb5b2f0b40f49077a433d4f5358feb" },
                { "da", "bf2cd07cdbd113181b37d73525911a83ee811682bc5c8db1c2f4a24fe7ef3c9bbb8f402af349d41706ce01021d8178b13016043e72cb49f4d564fb376d459b74" },
                { "de", "c314b09fa966d71b7094143d78938815c7e2b6e70e6a21beae740587dc2f9dc82ee0ca2c1aaa9c736123bda055f0d7d91b29e50bf70387983048755075d933cf" },
                { "dsb", "f37307717d12008bdbf4aba98c60aca0302d760dd9cd098124d209ef55a0ff02c547199cef9224cb4dc7d0513e5e0f20dab1aa2f514cca6363a602ad11a11749" },
                { "el", "88e61705bb2a8fbcf00c0e1b8b4cfbce884350b86d4c73a56ada3889f05c89baa029b2ad832948b09a12b248a506ea0eb6ac878188d642375bc7446f46b303e4" },
                { "en-CA", "b3e6c899879958195275429ca3199b356143799d48062307699600e163e8597ce6bd537e46ccf607bbaefb59c531d6026c9fc93437607f6bd43640ad7407a8b8" },
                { "en-GB", "840f37c015dbb2370098b619e9c14d0353900dbf697adacce824d7dd7f9610d3896c7c084b7947fe6d625dfc1f986d23621a1ce76e513ba23bbaefe490b2fb01" },
                { "en-US", "ea3bbc160fcb1f639b8e05b96c5d6dfbed8351fb9f9a575fd75a48ee094c7b9206e8cd1ce77a021463713ef66944cbd4608507ac99348d36893cf5bcbc2a6cac" },
                { "eo", "e63d99b39792914590d2061d148c43c1f49263b6e0645a2c440c63517575730798a09c302b6473cbc53b0756e310080b08bf8915c32bdca8d7f287da4f625ea5" },
                { "es-AR", "3a85918728ba026fbc4b04f357aa20eaf51e406c49ecdbee11ae51b07d2839e5c76954d551a09168f57007416240b9ac874db03672a7f3c7fb4eedb95dbf4cdb" },
                { "es-CL", "1873bfbbb100a05067261b817b1908427414f72e8bc36c764593fefb6a52db09eb22750092d77bf60c69876cdab2f4408f5cb5445128a10b48ffa0c6ce26d590" },
                { "es-ES", "bab0918c0b0105703099d46c57f9d0fab1caa9b8f0c60554248c4dbb8a555993bfc3b13a3364e644e6481e59b689f56f8d46aa75d355da726fd86f86d5edc2ec" },
                { "es-MX", "833003c69f14648dcc28108d6ad3c2ad2855c3f0af5d292ad8cd60be70dd269d54df6a1d34f3f0ebc411fd70f8ad62f7c1190e5211c09fd708b9b0ef5025716f" },
                { "et", "5dee237d6c288c1e4198d50739bdc4e3a3922a57149e40d586b52a8f1184e52b2e788f33dc396bcd7d56a05f8f4ac65c7882d1909c8458c13b21cf29e7439804" },
                { "eu", "4b47be7e66ccf5f1bf8a2bd1aaed0d0a272f22f111bb7beebd2a55bf4a0dfc1f29e476db04d82b0bd5599dd4b16620a6f5f02e3494df741df69cc6e627c393f0" },
                { "fa", "c8cc7a0a9f415eb2aa57bbb2ae1dbbd5147349d3a58c77259ec3258693209c058b4308a4c1f2a7f4d9c6f096bee526e71269070cc10b996dfb70b19baffa5ec6" },
                { "ff", "22de43fdad3efea9e1a45b1f2e8e085541f9f702607f3425064b1bbee271644f85afb1e0d1b4fae90a7366b13af630e115a001971d323efe05b643d438969a29" },
                { "fi", "d69261100966ac2346df7b59cdfb15b0bd96e78232cfe1dc851ac267b0ff0a79d627dfb3a175530b677bf0ac804f8cc935a94985ac7127a13f99abd21ae9a250" },
                { "fr", "8b55feb9a604953b5156ee634df84d8a11f206c8d16162f2a10677961c6be0367648c7975a3b29434cce4ec810ba59d6e5e4c5fa1a0b5a1d9e41d07a2c02cf46" },
                { "fur", "c0f458c49092f2b7cdd08ed096b0ba6e9b58dce9ea0f32315db02ef659d49bb1e5578bcdb2f497096e0ac501b6f798d503df613c82d63427709b001baeb381af" },
                { "fy-NL", "1464643802fdbaff72cbc8e91bba4936f2b8409c4f920e48d68c919e8fef005e6c102e7b63d242402a68d8c2db1101a6d3f5b85b48277a54e90a2f8275fcca94" },
                { "ga-IE", "48af80b0b10c49457045d1ecbe47d0e90a031d3d197f28f7f2a3a06ddb61ccfa2921b93f4f1ac8c9926066a2027d68bb6aecabc169b08f8fe610290b5a8bb29c" },
                { "gd", "7671820eb377a3eb5e108abade456185078f78428cede7ad4b189743d70285481b5b9c49864a63aebad915862842a1c4f194e6721d6888a124fc786c63e02fbb" },
                { "gl", "79895fdfc01b4bbe3b454a56047243793bdeae7919bfecf186953f428cdbdb7e1da47e765410e7be2c0ddea0d59848b2f0c9ceec3ecb00712a1c00818188c3bd" },
                { "gn", "dba7c594d4aad5a869a2757be379868b434a4f7c3a2991324ae481f9987d5dc9bcc5e8f70c41c89f6500cc457cf2a641a361925611998ef8f6952fe80b284435" },
                { "gu-IN", "df558c24146ff70477fa9a9c57cb16f63249c25718c603b039346efaa291340417725f361a65050c0f8ec3567326ecd957be8847e6f3d7b58c84e0126d51f816" },
                { "he", "62d665397ee622eab883156427863d21b2160f14b636047c3039695f38b126ddcb966f93b55f3e60beb2802a032bb4e6412ae56066dbc366f2822c94f787b399" },
                { "hi-IN", "4de01a7975d2498726a3522953649dfffcd1ca81744ceb797141a56cef8f80795403d91800a36a951f54793c4e56c2572fe1ed9835102f7a701ee2988630c6a0" },
                { "hr", "d51373baafb8e4ba84eb5945e25f413f9717f239a0a0a1d41ea19844f3bc2e51d0ac0f8e3e030a2046cd7c5e31a7703abe6e1ce27c8a2ebb4013656f972a6361" },
                { "hsb", "4075625018a1baebfccf3b2e374467a664caeb25d4dc80626213995c8c6c8b25ff1ab39629f44e3b34be2259541c361ddafba852377c3b913466bf01b63eb512" },
                { "hu", "5130fec11ef17b1af3d64dba8d8001da88266f7ec0ae34c3cf4cf03c509021410a01e8527c8b748701cb57125fde5698830d3c57c8ff073ce8fbc4bde632c5f5" },
                { "hy-AM", "dea7cc205c74892dab79f962fdfc5e7552a9ee99bef95ca76819a653840534b7ec28732dbae3cedc547e916632943afd306d2930dee80ca6b7957ba6538dfa73" },
                { "ia", "274ea388ba5055ce3f00691aa3668d2936a880fcf891125023e52b7563e8f9cc3dfd8852926b0c89ab05293e13c164c47a972f0528d914a6b9ab716d647a3ca3" },
                { "id", "3a5e2a63c15b2c5a76b4b565476499fce5a381b6dec9bd773329cffc4195d8a2ec6632b0159490f7c5d484cc9d1eb151672d959f79b5a64b7b7cdbd7413cb010" },
                { "is", "c8bcae6e297cc5d6dd4676d8f64269fbb75bfc708dbcc79e843a3c903d11782a1d9d5c47d84f16f818a2efd02830fb9e60f861eef799b8501d5e74ed2b33fa8e" },
                { "it", "72c8562ac9b159cc8973671c7a48e38d35aa7087a7bcaf446c333aca45875b269ae5138c50b56b86ae7f386e6473e48c69190dc2acc2e51e66560952876175aa" },
                { "ja", "b10c1799854ee177c033f2be4a9ee7214c0772ac4270fbe830188c87d3e8da4f1d64b51242f1f7c2bd57c6623de272e209dfe3e036141348870e98ce6db093a9" },
                { "ka", "d9b511ab0908fee41db453eaffef3c187952e956f6f9393af1be91b539c06a1effc53a12b5eda0229cfe5c0173f6ccf2ed5a224a492db9cfd1f195720c3f8cd0" },
                { "kab", "d1279cae13919c30f0a18e2cc52aad0ce3419a730183c8d666b86c0bbb87df69bc12ddee610fefc595d1c000bcfe9ecb3729f7cf188bf296199d7199badf9030" },
                { "kk", "baa34950600170b54ff5a9dbf7ff4dd2e4e5de017e354e585262cd3c511530607f7ff5a944a761aa34be970b333704187c32f944c6933fd8bdf465e6685c1211" },
                { "km", "4e64364b8f809910016d66b3afe20685298f0b5943c87d9bb9094294f6651c25b4052dd883bcafdb2c9b6ca9d1ca40891a9bbb5c0cb02f177324fafb4db28ab6" },
                { "kn", "ea9c70e5eecc8d6480fd2a21e1bad365d7743f9825d2dfa81ef67890dba7ee765a7e1c2677890f2ec42c0fdf856963f423297a6fc464b55d569811b24c11cd72" },
                { "ko", "d2f3d01e1197e4b4916e6b3812dac72b48e710786acbf5e58601dfce27e029458d19b1a5c4fd792626fece752b1918c12a9274f95af1f250a2a0e7c2ac18b81a" },
                { "lij", "b24c0e38b099908680e5abb557bf5e5cb1d6f517a9103e1f53ccc13167927978b1a87d8594a998a2f433834c926f187ba76a2535ba003c8e9de8010cd823e7f9" },
                { "lt", "ab368086741f9514dc7a61449f943fb9c52dfeeb42703d71241c7cf92715c825016b0b119664d3d7c8e279c5599f5a4553e0c63b2a46302f16c1da76f9791a97" },
                { "lv", "0172b6bda229d205dd2b85a6ee54fa1be179dea82c737264954129047f5aa925da0ce75fd3db8d7709b5d0f835860473be781683ef22236fd4463bf1c29ac0a0" },
                { "mk", "60f57fbf11560f81a2930cafb2da2785a4364ae55d41cec02b2ba961effe4e9fafddff20c064264c903c81143021f7599be143232e59d32a0c1a69faccee680d" },
                { "mr", "99c130f2dccb075374075af6b51757040088f18e13e2edf027441d7f16421d376ab24b891dbc3c8cba1505aef07b57e1d1f99e40a153c0887c9a6bb64ce44bf4" },
                { "ms", "684bb96025aa407e33bc268349f50c22523ba0f6f3ee94f99f38ea8b749cc5dd0ff9665ebc4812b91c9a69a89d6247f8eb62d093f8b92d0b7279f6686e9efb3c" },
                { "my", "9eac84634ed8bb326cbcd0e73022b0347b56addefce9c2590abdf6073d21be4f49577ed37e9137b2c5f906942410fba521f7d720fcdb9a083d0e90d4ddef90da" },
                { "nb-NO", "29fb0b460b12a17fa5d36bc4fd89921a36b676bea4506dadaa403c9e4797d2c02bae63a5dc554a3d0e02dc424fada0549d9339c7a8bf9c04e11c11149d88a2dd" },
                { "ne-NP", "cc9aab65efad0210e81ca9548cbe2deee60ccce64885a7ebdb7b218c13778487024b1425516580e3533d28f1d93ff671177fc1a58fea96ee60455b0fdb81cb32" },
                { "nl", "a147acc246e9120dd914b0d721a999b7bc5013df5536ef0bea867518726732ce1c86ab3422cb8ee54ad5f996683b21154c1de33233b764ba8525db8f151986bf" },
                { "nn-NO", "502915f651e321afc0274e04cff819904befd0f60060239db864dd370f5e5d53ccff4bff047871521abf5347e93f13a05b8d410f4fbe938f206b117d2d59a063" },
                { "oc", "8f3e0357265a8a0b7b244ae1c59a6bffa6f2b175d47045816ae3494f276667a8f57fa5f9700a2eb8dd55e4f8df02e0fa84434d613dd1e3c25109e75b47808d8c" },
                { "pa-IN", "b3eb6878bb832b0e2ef3c021cd119bfa975cad8be11ed31efcc1ef95c8f664a47a37498aaee6431cda52361c04776a90f137f72f7768bba10be8569afbdcf37a" },
                { "pl", "ac9927b8c796f8c18e96cd32b6cd9dea219f559b3897212451bc65c78b5c495370ca0002fb875060a98dd7047635529955704033062064e6c3af9000d339c272" },
                { "pt-BR", "2bd57312a29b25ff435493aa15f54a907b80d6199bcc115d299213cbc4db959d5677de35d10a0cd3ef710d07e72fba50f2efa6225bde151fd40b92feb4b9a47f" },
                { "pt-PT", "d7e218cfdb0b658242679a727a88a3299e0cffb1f110fdc5620876e027dc2e28ba7d6ac484dc1a5e067ba233a46732c7e7f1ba96665f3695510570542b873bc8" },
                { "rm", "606344f14d6f471e00c5e1c4aa0aebee4d51ecceadb491c9722bd607cb9d50d587b91faa6129f5dfb3d3ab6880ccf4bfea8640a9e57e8d96642e82693ff635d8" },
                { "ro", "fd7b9196da16e7946ba38fe1fc80f9633617b29b2ff50a8c717f5342227a3d5ddacbb9b2ef59de390748cdf247223aa2a43aa7952af93d86cc4cc51a8e4d69ef" },
                { "ru", "9e26b3955b5afad2cb6abfb2be48e6795ba199ed80bfe761033eb64e26b2e88360f8290c574a150f51d648222ef2b0d895e4871d217453c0fa9df59f5114ac15" },
                { "sc", "a276f6b40f59d5f8076092f2b3d31cb78cde55dbd69141bf4c8e3a0ea963375ffd290c0199f0918bfcd6c5cf24107454a786db890de57062a50b7c203aab512a" },
                { "sco", "dda832ba8eaa8bad2390061dd70a4bd9831bbb811d433cbda0a34104331c3c1ad339336262c667c048a8d5034491cfba11854ec3185575e6628d65029acae6e3" },
                { "si", "0e74b0a39d433a4185ea4db2a0b6baacd76700c23e86de7f8910727225374a2274abe86d91190ac28b2b03272adb479e4677dbaf70540b38eb2e7bfa19e68c50" },
                { "sk", "bdb0c43654ca2ba2fa6d66610b6c381af77648e33738ea1a109c1d2010c92aac3a36540174c1a7527dd5e9f7d3d267572d7cee9ab90e59f502e9fa85f0a5d11b" },
                { "sl", "c72828e6a9c56fb1d1b44211c6406bfdcacfc413bf0e16b24e2ce072b3bba9c231fc8656f5058fb584efd80402e2a3e7aa4ca55dd717fcba18726b6c6111738a" },
                { "son", "8736f5d2468fd3ae3392749a56a1426c6b9b69d95407a4e5cd59fe2d4b6227e1f7255b6adee2806aed7d25b30d909979641154eef89a650c07317200dfb62031" },
                { "sq", "f791dbcb8cfb03e833c9afea0ebf763e198e3e9c3bb0ae77f9120bb279321bb2c8df5ae81525652e2b8ba69e1575ff05bf3893fea9c7de0fb60cf1bcaa8587d4" },
                { "sr", "f0abd19a4fda5d4732a47650cdc94c88fd24311040325a6f5c9175f2745b570969c06c9796b5166681144d087d2b11a0dcc2925cdfc83682fe1d9eb1c6594cd7" },
                { "sv-SE", "7e8ca4faf34a4bb0fb2cb46562356d7f18b28d4cdcdf253712a26e8c0218c3d70c853f11455b6159ff9d870ab8a5e0093e181cb949b705309a38b3e18e784ec0" },
                { "szl", "960e787b4380434a9a99a8a08dc27ade870d588dd15d7b94dbfaba936376b5e73f7ee9e4ed1d46136736abc95cba4bbe3f37ee2b46d13e479cfef552d76797cd" },
                { "ta", "6d855816a53eadea87b374c51709d363bed32414437e4650d98f47432c9c68506bb43a08f71ffaf86ff5f5ae97e9115da5ced6dcb8ee32ad4f31996237a3efc9" },
                { "te", "2b1e9705ca8ba85bdd9eea3c8f70e7ca6aed39c8449c32a6168877920b3bd4bf0391b353e6f85192507870ed70135cd47f0c3ce6561d94e4ea5fd5f376d380c7" },
                { "th", "69bc10ccb0325592ce4b9cbceebb1b4f295585c53210b31885483e83ee27852e2b0195258ff9158090bf0b1a5b608bbe70755f9a3fbe09f32a097567aa05cb6c" },
                { "tl", "5e4300bd8ecafcbc287c6af7b9e9840c0b17c3da28d293d334028014f1c6ed9dfe3b6adb109798dad11777329065ce2741648217f86674dbeb89ad354f308b87" },
                { "tr", "1a147e92217a9f742510a6f78679a21e761a74d22d06ec4e3c315785f309a1d8167a4aa054f3247b0d06d172efb66bb312c0db4128140db65be32f4bd4b860cb" },
                { "trs", "6571f09f580332bec33dde8a57b7847d46e42cea98f27ff9084cf788013420cfb5bd9e090c81de532ef4ddf24f66877ef4094d2f5b15d742c9405cc279574d34" },
                { "uk", "eaa25c078af984f10b4a708feaa7fd4306451d9c28aa35f317f32e9b846ef56d2bd22678f8b6683bbde54bac071a7e79547c9a0854bbfc8d6d386488bab4f1a9" },
                { "ur", "feb00769540f085c030a372ef23e2fa69762d0db0c4370a69126556ed6960e1879c7c16a35e94e4230ddf58af64f518961af975799e83101701361403c825320" },
                { "uz", "63cd4536c251afda7656e549fc29452ab02a6228f28480b16b317b22334f39a5583d7c8923c9c90c203785b1ac63c7b8d2da39a0fd805de88dea4c6eb99cab24" },
                { "vi", "27104ffdecb4b3a3d3c4b86919c03ca46f4cb181ebf055f239342c70ef2db3685586f6a3d4a8eeeea4fa0cda5f892e7f16878c7bca0212171bf56a0fd5699fed" },
                { "xh", "01f6558370221504d362e41b80cc3aad3cd16df2f840d594488c9d178a8f7aa5ce107323aeabe8f639a50e72d8acb6d8151682fe0a123391ad40c618e29537db" },
                { "zh-CN", "ef135903e9a2f2b3ba9bee546ebdf8d617df332847f6cb1f3f860179c43ae03329602120eee0eb6903f68c024de3648b7ee118ce200a431da13bd44610d31b90" },
                { "zh-TW", "155450dda00b5632503366f9f1285ff4fe4b423bf89a0a2db22a9f2545c600fca68b3bc5881e3cdcfa8e961a4f78a227f1a3c376e22fa318d00681073cf79765" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b5/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "578e656773bac01a692a20ea3914cdabcd8ce32efdcdf8dee62f9a2fd66248e24086a742fd7b0fa9713f4b169f73eb5e593620e0442f3073d1edc6505eb383d0" },
                { "af", "504c96f3ba29de123e45eb5c2f6db48e9d5f70f3bfd4b578c8e6343f467958982219737e49e31276e44d479ec175d970886ba1e58d0454c219c8665df847f6d7" },
                { "an", "b7fe5d06a2d8c631d43eb68b0a273850ca7c1065e93656412d887d93c12ad403d02994e7d8ed2f636c66eaef77177bd587805982e319a9d75f3efd42951a2c47" },
                { "ar", "ad5b19c4d2a3d5a4009831271ac0d681bd377c204d84f39bba76bc8cd624767820b7b0e9c41d51ae1ee2250a01210cc2a962b93ac4432b9fe444aaf737beec81" },
                { "ast", "62f8d6513593c8189b604cb216c641d2204648c6049a4f71a9beb8b67df72da5030ab2ca7428384fde1c457525eda1d71ad56552301d4de1a0f7e5f07118d4da" },
                { "az", "dd1a539c96521d49024f6c37f6ed48e3ec7c112ca95be9522f2be98b511585557a3d3f6203453ee36ca6ba41f9036f87d04f58446bc13eca397f74f67d598cb6" },
                { "be", "2498aa06db046175814bc61369d183112daa6e88cc65f7db108c68201e6de41e5a48662a4d4359b4db7546bbc0b76c942acb64c6c8ca222c6914bfce310a4ffc" },
                { "bg", "455785a0790e257d40a28a3c8fd59b4d634a06f380be9682341fcc8f36b6f8cd677926542a1806d7c7d44f1b50410472d1868f39292849c366409d2be6d8cfb5" },
                { "bn", "47b03cd60b0e0d82e86383ba2d34788bed6a3ed7a1335560ac52a465628fb274ec491d186ad09b84d56c3891e264f6a9289ea7ea9816c49efa2e5227bdbe56e0" },
                { "br", "4abb8c6692cefab0efcb261f83c3b82df688dd1e782cb752c9b3228d968f1e62031b14ebb25656201d85e37bc4b32d5691fd88a85fafc9e2a20a43848abc942d" },
                { "bs", "466bcda8a867dccbe7d1eb359a93b93a656ab5eea6925328d9e06d23ef657dcca9cf9607a8187b059e27413a4b1d06419db3d23508ca0787ce8e8f2b51ef23ee" },
                { "ca", "e0aa6d97f7e72b1770fd2be60d537782d5970a3a9c13435e14f1d7caacdd8d6d059d0bf48faaa20b64ef3aaf88ba3b9d2a82858938ac3fbe59a392791fad1fdb" },
                { "cak", "08b87b735ccf154427070d9d941cfd28675e449e3ec6807003fd90b51bfa04375e5eca1048e10ca5a96bc0fd975a56ed1c38fa4c48e970e7e8cfc03f22c2cd9a" },
                { "cs", "2015cdabfbe999096875390c766001feb34201e7ae40c979b68cadc955c0b8dee478f04a8789e1eb25cacaccb97c7214e8f04740784b0d7581ddf9a2b762782a" },
                { "cy", "86f30c89f10aefeae98eb9077f93c40d4fa64e090f8f5d36bcecb6aff37345e6e4431e46044f5aac02d28c3a9342c69a84101b4c93b2d4204f146463233d46dd" },
                { "da", "d62524990fb0753f638722385fa90c32e3afb04ded755b220ff0e5adf7067d08642cd32fc59d0742fdbc14597f582f9a096bc0f4af1de7aa756a2ef67939f01f" },
                { "de", "83766fe30418ca7d0a459deda2bd9d8df1d5f60c9f98f0edf4fd02a9ad8d2c6023b15122c5cc5e89d1a8e964838b0849484833af050285f3e5b13a1eaf74e8ad" },
                { "dsb", "a3acfdcac7e2c362f7b96eda47e86552c3e5d571879c97d01f9a9bcf0602af7707741b58c544680f58be0cd9349289e3e555cf687dd53f950482b01567ef7aa3" },
                { "el", "adf6b754b3fca8a8d21fdfa04b86f00bc575e5fcdd78eb06b1970f0947c0dae2456db847fd53e32553c732f4165e6bfe85ca085e8deac7f7dad7d9b5d1c6c64a" },
                { "en-CA", "a291af13ff7894de85b1742abbb8b6c284e32fcb249d4a43daac379139d7ad3bddf18582d606ac5da5b20ab3c69473c8c5fb5f53f4074bd7435a7f5156522e70" },
                { "en-GB", "3ca247a424954e398450018641f7814be66f97844a7e72cc7c6513ff3ec9957231bbb33e6aa652f8382d5ef5d50534710c1556e79957cb09b35c6cc5aece64fd" },
                { "en-US", "b64cd0d2327d9cf33f143eabfe674f66121df8c06ba4b60594a2779d35dc3a7c1329b86be3a8081abe6495f4e98d4bed490e35020b21a91e543c3efb449991e8" },
                { "eo", "a75c47cc6e02572a84ae4b888e04f7137ab44cc3b14026aa7a88b9deb511aa250e248d492c510f981b71134d10736cf23109c36a6d9a28e452632a72fc850498" },
                { "es-AR", "2c1ef2c2d2c7c9b7ba7915d60176f28b4bcf59e5d1395eb9ac5f1464f17084e1680827b253952578af755618fdeb4a28f474dc6f349f1e324b7413d79d15886b" },
                { "es-CL", "1eb64376a4e0a2600355c9adae06b0e5dd599283cf613875d7d027b5008bf49dc9e098192499c57d188821e2030eb51cbfc64c9a293a7fed0d6851cbdacfcdce" },
                { "es-ES", "7642b0d9aa1c0cbaf390e1662a7161b80091c1b38575f15d4abb838e5210757ad4307d280cf0be90cc5ca8f0333421c457127034a62cb421c47414b8fa7b55e1" },
                { "es-MX", "a087679c2ab9e4bbf2045d55efbedaa8be71adef4b2f2fb647272b80326acfa41ca455229cfcbe771685eb2ea3f080586ce471ce7a9d06b0ddd6148c91532a43" },
                { "et", "42b86b92cf35c6dad06bc1bc202cbc8b133416b6871b01c17be4c4686a8866249c89a6a9f1233fc145f08f9aefce7043f5af4cb4be97aa40b10dc5d95767e9b5" },
                { "eu", "6f1af2f6c3bc6c03dad6bc741d2059c9d899c9b010bcf63aac23e0152eb4ec6776608bd89eb183e4ae29ae8c434f9d5e4132d234cd53363ae1dbf0b6d13db67f" },
                { "fa", "cd9408e4b40108d5dfa1cf5cf4ca55e0b28f5512e3c933219e254dafad64d3db5475b96c4a2bc59c80bb670551af7bd96393ddb5c82b202e9214b2a224e6a567" },
                { "ff", "97079b5daadb8879e897fcb3126a42e1327e0d3fc2c06c81d3d588d17b282e2bd085a95d9cb9213af2f38c77db1cb091f43304e75efa60f65c817445c59e1fc0" },
                { "fi", "fbd02891ab7354fb98c60b0db32261991e1b9c494e56ecfa3c949a7fdeac74e1db394a80fd0bd3e1c49f9582a0fac745e245c4dc5336cc8faa0ba7e92f6e1b8e" },
                { "fr", "12db1e728de9fe0df9adea45147af464bf01cd205879bf974d9f7c1937158b9ceeedff05734a4421fb6e537a2c6855af335272f5bfc2f031601e437ed74c1329" },
                { "fur", "f58645b2b98c2e54d9c0700e9509695143ac2bd5765a763d53e859940bf30fc07deb78c936466cd605d1e740555ab15546e0f15c570be68aaf49fe272f8bc427" },
                { "fy-NL", "75a0f98c674d6956ff2f24cd4d03f8f8822f67ce938e3ff548e2b6cac037380a168c307d16aff32a6b9562ee2be9c59128c053f78a1d8373fb45df527406b36c" },
                { "ga-IE", "2bd5dee14920dc1fe6cf46109d56fb4384b5586ed5dd4f73dcbecafb4b2dcfa6eb7d8a6800921285db78bdc6e1168469d490feccfa04ddebc07d1e90d327921d" },
                { "gd", "ebe4858c9cf5650a448951c72f5add6612d02440f8aa44626267e7e69b9200b0e023c2f1bfcb61d5aaf1dc204c2d50ae8b61b4421b691f95e71f97e6bd408cdd" },
                { "gl", "c46db4fd3c433f8e1e53c1c160d388171cd03f430bde32fef8ed36b392595709dada368acef220bb448bd51947658b572bc927cdc6bd0b04b464720abed7ca91" },
                { "gn", "bde807b92694a887ae56b40b54de9e8cc68800d08ca2a2ceaba53db70d82ade93c4ff4ace346724298732ef32c8125b789b23db38dbd8cf4aca72590cc471c1b" },
                { "gu-IN", "bc810d542bfe03fd77aaa2c4c820745cfa29367cf8cfc0fd39c2e4f9459bff0b152e1e643777124ef850ce0dbd6b3a9b8506036935532af392503b9c70939f59" },
                { "he", "3f7c9972a74024538b966e83d3503f665b07834dbc481ebd04d60dc28d869f774c52cf3b33edf71a66cfae4744e41e43c9cbc4bb2088d311b4987f7d5adb279c" },
                { "hi-IN", "dc8b0b4e94e5deca46e92fb52b15b03d908e80b205e9b86e7eab0ecae88d76cd4929e25cf490258077621756488b96122fe96b8b9e72b4e1098157f49a98f20d" },
                { "hr", "2723acf0b30bbae9ae0b11406b7533861d3e03d02b4c1a4d54a0c7733603207e30374b356a0cd35810e9ffd790db71117fa154e4bcc3ad28612c94fcc1872b59" },
                { "hsb", "9fda9efbbe34b5be056f7b4035a62cb5032c39cba1cc5b360bac9bcc5a2805d90818feffa8d4c194caa8e6a0fa5404c2e72e1c30479a8ec39006813baa834209" },
                { "hu", "689ce19f3b42aca8853182126b3001911debfc460d5955b7e3c6334bd29900ad890dfc238b957371fc0713c2b7d4dcdcc88dc64d8f4899ea2c9dc1480f9dfb7c" },
                { "hy-AM", "a020330a95e0125376beedf107c6dd2a77c7ba2d4d1fee94e50c90fa915f7b82a146c8b52ea491ac0a7972bad041b615d0920d411a64473c806a959251f27924" },
                { "ia", "b62a3ec502e622f245ec8633fcfdd7f830102b1a9a967f272f84d38af68caa87f0f8f57cdf0316b7da219d02325f1e43a2f223a3c55528ef95bc2d06298639a1" },
                { "id", "4af6341f257468eebad4be1c14622cbf4b02eceddc3ac6765000a9ea139ae4e5c06ca54f897492eab8b31b8e9203fdc1181e276092d881b17d464a1a6072ad0d" },
                { "is", "b534ae3aa2b55f059cf882f1207bc6a694d8fc6ec12aac93e332abf4e5cced0568516d92311d5f6f5493a8bbccf4f441057b346a97c7a71d1d4a58945c98c2a8" },
                { "it", "f36f4471cffb28599e03ccea07d8a712f3f15ddce177c09477bbd8d547aa0d13ba3fb3b7f8c2a1b4700d0fa0623b2172ed423d14d4fef4099af154b5e789d6fc" },
                { "ja", "8bb464c2d3e55ca69f97fcabf84b14e55be7e30df0d19d0f57e87159eb6fc079cefe175dbeff524ffaad7b910d529ea67e22045549a8e60bc6037bb0658500df" },
                { "ka", "9189a6ee1dafaba91232a20bcee5a555345b5860e537fc178aa274e610a007403c8182fafa5793937f9ddd945bf086ff0f1f1e57be45d30297a5c0eeea447fd8" },
                { "kab", "3390ea7b4173d941ef147f1138c6cd546b48f115e57d36047c3fe85af23eccd4de69754fd9cc25938b8af9d51371d87253691fbf8a223a3d4abcba1404e689e8" },
                { "kk", "0f7df2ee210068db4534613876326cd724db7f52073e56e247d6848014624c266d52f2e6cfc73287e1ae164b0ab9aa6714d459819752839819a9b5431a403e45" },
                { "km", "0122feb3400a4a4936464e8924214d5e034860f1d7b2b49fc6acc6738a404775f864fdd750de5e39a54b911ea03c1806ed0293e8f880686b4758c8bc8424dd02" },
                { "kn", "c159dc74a59ddc2bbd4bc0369be31c120e638feb2b48e30ee6bb186d8c72f2a85590d9979541c4b7d81c0bf59bd5dfbfe3e12b7e8c84d90f369208a1575a7f98" },
                { "ko", "301e61327fc8e58cbd2ec8600cf1abf0cb1fca22e7846939b0474ab6712f4242f0519c5123d0208ede2b63ea355b032338cfd398bdea6dabac83e31774a89bff" },
                { "lij", "e7230ed42b151a0a162a1f36954e813ef32dca93264f37a2518ac8f9c437f946ca5583919dc1c5984b37386101b1f27058ceb3623b79e42984e47f6e2e0ec986" },
                { "lt", "1c072e76ecd8dbff83186d43a37b8718fd5293d17ab5e6fb60ea909b807e83bc735e01c414a83abf3c820690e8f2cd3b20054b85b267cd57d47395bed4850ce7" },
                { "lv", "1fe9d39eb3570b57a7ddc165ca02c350b540152cd6ceae586e6fe5be1f92e7a96d1e69fe5c120719ad9eed030a478cdec3f0cafcadf33560636954e3558201a8" },
                { "mk", "633b51d16e3d3a03d8d802d4483c31218ca223b15be9ca85e7b6df0677579fe18a5555de78f9d36bcd7b2129814d6bed4b898d398b02cdc08ebddded2a2ade99" },
                { "mr", "8942891f69b5d6f01afcc2cb76962122abccd3f50198fbb73872d9c6c0dd340bce99bb9c42ca179c2d64f0e54fb08f632015fc787d6906be7327529f1cbfdb3e" },
                { "ms", "3148c9acb0641d5cd985829af9219042045fd46fbbf33f24e9cefbbf42b5cc821f0b15c9627e08bc1f0a664f74a4c20b7f1bcfca7cb2ceddea6a68e5ce88f56f" },
                { "my", "d822e5454452181fe3dd7e21048fccee7060626cdf11425fcc21cf165f515f3643093e338dc7ed6d3f2a35d8b9168258438c6f720333c72dcba8bc2e06236787" },
                { "nb-NO", "a655799d2864788e48b4b34a104b19111f225bb7d83eaf3654234d55d4356bfa338df144a1014de37bc4cc03716e088316b394c3ede548cd11e954091d9b0288" },
                { "ne-NP", "3d262991624997258f5325d3bdde663a6bc6fc70925db00f629fb9829ddbc02a1ba31a10aaad1c04ab8b1b9b63c752d1d19984151a8638d6adea6db684470a91" },
                { "nl", "94b4287468a2daa0da02f75e60562465e2411ec7ebdbc8c5d6da97ca355420ffa4f5bc01fdce755187bf655ff9a0b553ca7d33350fecadc00f55be213f05f8ae" },
                { "nn-NO", "74c385f1941f126e53a4d1fa6c3b347d87b562961d8b02528cf0c2064433fe0b00e1209a3a059f04ae80cab2818d63c27e9ebbd630ab3d4d141e6131bb50a72c" },
                { "oc", "79934208e62ef030190993ca1282df81d08bf3fc256fa704a4a1225163249d5dacb7b8bc344c211ea143e7190f46bfae856ced3359c4b6bbf01c997637eb4536" },
                { "pa-IN", "67d4c4c8426319cbebfa27c3462fd30c978fcefc78ec5456514ec34b8aa3cc128b3024161fbb39c71409d864e6270d531dcea7af8aa56388d8e57d13002ee780" },
                { "pl", "947c8cc0959ec2ea7579eceb2c7002d923af2135483c56f41a781f1cf947c4a076064e0c2cdd887f576aaf41043180e8c46e4f16a622c5f861e398cd1747cbdb" },
                { "pt-BR", "9b35210c9fba2c19f7a39fdf45d2d086e2859843d5e3fd169318bafc6cdb3b545d67df5875a1384e3d179ffcf83edd60581144618d5d2d858de2fbb84551fd70" },
                { "pt-PT", "6e79a78d554335f406a02175630eb1b11be46e659f071269431dcc60fa470219eacd55c0f7df4766439fda82fd1b24390fbac1683d5899238ba1969a27301efc" },
                { "rm", "b4b1fef44473517a65530deb036b4df8690d345caa6b2e4ea2b8d219b0737726fd865a3fc5936d0e2d93081b8f2e0c42ee83f1760dbb6279098d1d4632411483" },
                { "ro", "a7e489b8b1bbe7f4650915e65350088d2dfe61480a5ca105d5e17cc46d56143221fe3709972419665a93f081b466c2085ff81a4c60310820facad626ee1fa536" },
                { "ru", "1e808ac91aa5e9a997d5e3ae1be8862479311e8b4964b73c6adef3df38519196df1073036aaa3a1a5216e40a5a10806d2de04fc5ca66407218488da94ae7629e" },
                { "sc", "bf20a2c29cf37b880f6fe37f43d2196da99e7ed49df81044e15811d927b905bc36dbaf5ab256980482c6c57ef394cfed322017494f2f6894b03cba406e39e8c6" },
                { "sco", "d18f3c304f18653344b603486dd8fd73cb685d2d33ee204291af1a286b69342e1fa3ad4b4ed7f0eac7dd5c1e2ee0648335fdbae122398df48f1d2bccd2661b8d" },
                { "si", "315a537d3360e6fc2f244360fd78962b751a4abc9243b8902352192657b8021c6229c698d7756cb3658d6c9277c883f9ba61133b3425ae01509eb1d4a882c125" },
                { "sk", "04aa602f11cc2cf9a21dae306843177a09d5b6a7704b6da87314f44897493f131e76bed19ae373a9f9c27c573c6cbff7be6ea89a3d6eb0e3072e281c71a901ef" },
                { "sl", "3d114ff698cbf433527e43a959c207a2e01aae46a23b6479f64dd0d3db06769a0e87ba09493bf2ed5416fcdcb71355213e63e3384725b0d8d8d4066d43e01cd0" },
                { "son", "70a21dbc0fbb9caf27cc345a09a6d4529bf054f30d1b586ecd564c94e420ae54053f93f9fbfc3ce2f366004ed585d43ce0cb76a5e1b03e769ff6e2d0a79a2397" },
                { "sq", "eec9cdc9202b86b11067ea7656db93c431dbec801da6a97bf17c232c88dbde1098bfcbd9e56959a437f7c1bc8a4375870e8c99231b10de152e2f372161de9855" },
                { "sr", "f06431f7012771a4534c07dcab7ac5edb80b7bb3c4dfbaae949fc85aa97110e8fea2d1bad246e57fd8c6a6e34ad2d1f192840bdb34ca364eef5787626d0a8b4f" },
                { "sv-SE", "e23a1143a80fce447992178c47fb33e22c957c393719df77235bd703d09c31c95bfc66abc21f415b6ef4c13256eda1b5e608c778ccd5e61db1186e68a7b47cda" },
                { "szl", "822af746727abd9878a2b7f686cfab74b97aed9346b0705de9f51595d9576bb9386671de1f025cdc372cb52fe05fdbd42e6ff94f68d189f6160f828dbe7e1968" },
                { "ta", "1b17539c553059c2464fc979dd6eb3538dca07238deb50625a92adc11c0fb7ea34ad97f37c3c3d142ced279b5666fef350f179a7134d360090056c0fb7b85d86" },
                { "te", "926dd5fe0754523605a0a117609737bf0f68794bcab09c0ec1b1cb92572770d11c72f4cc3428069ab15b9804a814945a2c2b0b97520c37051892adb09a9eddd7" },
                { "th", "64d10a369fd743c755e6d734acab037a82bbeeeb78acc528d64e9f9fe0caa6e735f26f84d8bddbe34ff368bec3d870fbc2fc661906cf4b29f1de97850b32a9b6" },
                { "tl", "00544f29de9300c144a08d0da6b0ecde03c82828cb6d9e87b733c3b411b290f54a3dd439f2afcdab5c7811517693c620a25eab5b6c0488566c05c1521867c4ea" },
                { "tr", "af47ca20dcb1a20685984aa5d7931693378e6866b2961fbcb7518c767e87d7ed60bc26c5ef90fa1221f3dcc94f9f7aefbf98b952af9429e6a10baae6d5e86ffd" },
                { "trs", "d92d98f474b200843ec0d50f8ae55f870e7b6556a7b0df9f15387249b49cb5d60da80472153b3c4ddf393cfbbd0c1e7d98497acc050efa17c4598d8f7a0880e5" },
                { "uk", "0b442c94cf5fff7218e49678ed612733063335c67561cd0bbe1299e22e118d763f0491221a6f47cae3b0f825a9feeef3e2d793a598da3f9e0b340da0c7877826" },
                { "ur", "f30afcc98fce7c1b3151ef190564760559a39893f254d79aec0b257ba2d7d4a24e86a9878f024dd3803cce941d502e2026fb8b1d498bae947b7c9d548958f371" },
                { "uz", "ccc77cda28167c23de72de9f3b537729ae57612ddc1304b7989e23fb2f81d6c49294126a66ddf920a9415cfbef0e90754c88bd6e11f6dc890ffce64a2535cd73" },
                { "vi", "d89eb51dd9edbf0532cd62695375ef4ffc80e2cc8646a8ae5f70b90cc4093e1cfe860b8576fad78985f293bd39c29025138ea3513c5532960d51458b51fbca58" },
                { "xh", "a4993bf137ca93c0e79401417a4fe77f067fa88fdff98aec2db126876bad02d06c5b7f489df64a9a5acd324f0d3fc8cef7c051fe8227b9e0813cc0047975c264" },
                { "zh-CN", "3869adb4fdfd793aed0a3aa2b3e66ac774a4d5fe544372102f52103f8d28f9faccd3528e65f35353f2ee8568e4146bcf9863c401a4c155edab08b2174ff6e3d6" },
                { "zh-TW", "d865fca8442899a42b0937cdf999aef6525899907a04c7f40e29d074e990ffb3b42814c82ff4dd8037c8ad9c64533e48436e6a19b8281ff4f72f9d425255abca" }
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
