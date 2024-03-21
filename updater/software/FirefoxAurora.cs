﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "125.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "486dbad2c5649c5f28c55e3d496e63b0f4ae6285a40ad57e7c140a9da6b0b46c355a0be43bce34be069a9f88a1f3b8e45223c417b69d28cc62e4f780e28dbfc1" },
                { "af", "95b2e0387f5f62882e3a44b94b93af34618e0254f05db62b46c5fd45cfe589e77c609e1e177156e68cbe2f80c8ca0adbdce8c491294e459febe1bd85c16000b5" },
                { "an", "5d61face50281dbc094192c6199c99d2adc145c37daf812cfd5a593abcff33742ccb6450d1e4e624c1c9e54ed011cb2d02e0c82396fe21b94f47eeb175fc71ea" },
                { "ar", "7ae2df53834c1a320be0f6036bc80528c5cbf3cbf958dc9de3e6fe51ea64fef93bbb12fa86da06c0235ee2598683da0736031b764d0c97a50531f9edc4b22f06" },
                { "ast", "68c7e6903f8cd18573281f834e460f48724fef3363e5b8a902d3dbcb579834752f894cb5f94efde97078a372ea440984f0b16840cb3ff034b9e55c02a9120a3f" },
                { "az", "e6beac08e40da0f00aa9ad1c52ef425e63a154542637d62662968d412345fd16950e3fe1956e306397b8df0e587facd727a9f2f0203547e5ee0fb59c75a98376" },
                { "be", "6189ee29b3468fa900f2d5df3a5c68ddbe5ed0b6df2209456875d1a4168ea27bec8941fc8b3bde129b7c50a758b26ee7bfded957b9ad6c359a9c04a6f0de6fa3" },
                { "bg", "3ed66bfd1948b6fda216760975a0ed5d0f35a957797993d7cd76d316c80cf604553ea275bf21587f9c5628572114fb0d6b71c9a094116c6f7cc7f0193e9d05c9" },
                { "bn", "ecc71b3890d1900c1b76752e6d26d7b59cb32a543ebb1b71c9095055c9812b18fee1d81ca40c93f2a0ca49d24b267c4505ee14f2590986f681de58b2eebfe93b" },
                { "br", "a8cf6de338bc9269f642f8b4b4a826e0bcdc1252a420bb2eff0fdb9a54c77c1b8997972cf757a96d25e3d404e17f58a6055b620f72e1cb869323b81b646b7685" },
                { "bs", "745f888e2e36ce45b0c0b52b8b2fb991086c8b835e78af38e623d008fbb68db443e81c2073ff9181ea6d82c3414e4bed4b06690347b2aba20b4ef67440635243" },
                { "ca", "71a773c229358b2999d6ee5786efba55656d0756beda670836234aa06aee833ec869e5552e9ed47e8a6f21a6c4a3a558edc3f8143fba1a2e58e44944f8ccec03" },
                { "cak", "5b4c80b3bf998f0a8e75f6fe9b3d179650760bbfc3a774c62ef8f565055ca1e510abe9d736b2a13628a8ae624b725a106f9cb9e01dc3c08b13532b11b34e28c5" },
                { "cs", "780543b27602ede42d618785a21232cae3adf0719b5eac71b747fec5997f362d3c20398b344bd086e523636c9a18c9ec84c067a3f10b908326fcd4969210dfc2" },
                { "cy", "e3d472cfaaea53f53a858cca9cb308aec16f5cb74c47034da4b64301f87fe666568bfd94659cad80056d6edf375dd75528bf68d5efa7e4ab6bd5e35f0063d034" },
                { "da", "895c523cc92bb80b368299e80516724bfbefe534e2e7c4a92d97a1c0fa0b6b53d8b0082609266b58f916513da1c428167136ae3ea9c93200b4aedd325f2fbccc" },
                { "de", "9b76fb20a3cf413f4eb0faacd2ddcc1b788cfd8931f83a92598940e66ddabac48a4a5e787434767178947fc508cba31241ad630779d31731c1d65c2c3a81cf1f" },
                { "dsb", "c2ca737b1508eb74905d40969c7feb1d179fa260b549814aaf0af303c7c68308ce8ca264ef0b9455587c7a5b71c7ded72d5f3445044b7199d21dab1bf583313c" },
                { "el", "1b7098993e70b1f2228f40f7cb45a2ad99d0548726a71734b0926ee336e99c2c4540eecb451176498552d660007eeefdd60550f9272a9789dc130c106a95c702" },
                { "en-CA", "bc43ff969952db8406545c6a91f2e9cba6d1d8455e6056127fae1cd312267f5354ad3cee1c06f1e96053bbde4b9b52bca327f4ea36129efb2016e1959cca5b55" },
                { "en-GB", "ea28dd27fa26b40c2961875d33bdc59ee6b0747b8833465dc447fa94196935cd8d0d2ce091c6d2a5ee02a947724706dba01d34aee6bd87e1970e6dd6cb6b728d" },
                { "en-US", "00b7c9090ead7682e85fe944c10fd3015865524aac74a9dd7c1b7b9f69d4dab18c911706d22e061661c428a8ed8d8c7b5db9a5d82e22c24354fb4d9117777c29" },
                { "eo", "3278482962fad9c54847a9d78bc601de17885b2bb6625c9883246e75b56748de60ca9ddbef06a2edb0b70c28a9a08c25adc49bf80a7b71fc0a079bb812cdee5f" },
                { "es-AR", "722f6496648d396d019e307e9d4d3d7c9237236ca5a82d07c1baa55db61744bb941058ce40a59d56f95b410c0c406ad8dfc2cab83997d3a04df7aa21458c05a2" },
                { "es-CL", "597f78dad806133503fd26180c0e6d778053d1732a9cf6cd9de54d46177e97571c0b158b1d773717d7533044164136d858409274bc7002d25b65ed2d7b274e36" },
                { "es-ES", "4caf094cafb01e29da746e99debbece755556115bfed074138a88c4ab91d32403c490a590697024b61a638472ea1e5ef81f0b037e3b2b5216dd072e80b260d96" },
                { "es-MX", "0fa95fbc1671c261e36b1d9de81fa5dc0f0c02f11472810bad6c24a706b012fc96516e2e9d8bed412973b2f927a8ec6fa8238b8ff8e302d2ebbd5269437b56e3" },
                { "et", "e3a7a70dac8441f7afff3786fd708489b938c4813b023480b4dc3a61018e0dfe61897e4f230ecbdf70a0a5bbf90835dfd7cf5336e46fadd3bd297cda6e32756a" },
                { "eu", "914ea30351750d6c34d99b719a88b4dee677bcdf05bcac384273b8f183e325d8b36dc09a1f0e5c0e3f804945e9ba2ab669623ae80cc38a285bb95d8acf7cee37" },
                { "fa", "dd160c53fd0158fd44e8a12d3520aad15d3ed02c1bd86ce0aa6ace6e49781d9efb63ab5c116888992618b666b03fce93a49d1da13f25a4e816d07c4fb742e8b8" },
                { "ff", "f8f1309e89d0783a65e2de849cf2b461e566fc2a6e948a24f19229cb0cb3aef638a237ff55aa6b6bf745dfd8b6e007e40284b7a7c0c09e2b707f8c6f1f8734a2" },
                { "fi", "de2eaab7d24c47d4aababa09271e448446887548dc73037eeccd8b4414e4f06a17385403770b5c0709e2cb0795ee1b3e7bddd4dda1f5920b54b5d838db31d71b" },
                { "fr", "8c02f7e79407ed11c00ea732192a6f9a94f82ba2c3cde692cdcf5f899fc8524f2ba5eaab385405c986f1fdf5aa7f0017615013d149c82374f5cf04c5da0849f5" },
                { "fur", "8320878d92a5de120d58fb75e501e1a3ce1411e302ff9a32ccb236d193a50256abd82eff8831b1af36f8ad90d50d929da7df056d9bfcdaf2dd852a61825f9a74" },
                { "fy-NL", "faf25ce910711f837600527b1421e3e8c17427e35bd684b2cad970a1864967755ed82ee9c365f2ea1adb2645c38115482dc606e2bff0a4e1402717fd0a05afeb" },
                { "ga-IE", "c1f26fc79723dae99d1617082d513c76ff55b354a5593ebb80c40333ae206b915732fd98eb7f89a22c7ecd74cd20592bf3bd241dbf80f0963854b50a21320cc8" },
                { "gd", "df02076ea108e161819a998bc87e98e99436e41e51488b3686d703367a07ac1adb0476b70875512b2a99275bb323b9a0466345376d91e820c50b8e61666c9e65" },
                { "gl", "b0209947ee53cf5656012799cfc8f097881fc381447047b243e23580eac2313d0eb42789fe8cfa906b0165e9c40d11d065cd69eab9774a9c90112e6669d7f38a" },
                { "gn", "df317c592b61e23d047c31de7aa7940246302d1c3f9cafbaea5f8bee25ed64f9e0ae1c8b5ad45f724cc275c54984b5ffa3c4b2768f9b831ca5d97666eec49cab" },
                { "gu-IN", "e880fdc43aafaeafd4fbb7c99d10bcc6b69b87223af65226a27acf7af1cb2191019c1a2a058529c7bfbc23e355f809881796d46f7e421f557163cf7a40402a14" },
                { "he", "6a9ab45bd7aa07f74675705e39fabc9ead6586a8e3d75d40d2a4d27129281d29fdb8f60cee00dd01ed505fc8ca5bb830e7f0bf48034fbfcb02c4407247b549bb" },
                { "hi-IN", "a612f1233147590d71450020469c8ca5962851fb231c8984a25acc7e9d5a6d91137c503164f51022efd16dd7fcfd5774a2a40ab7ecca32f267a029d0356ef5f2" },
                { "hr", "a212269ef2820c04bd6c8a9afad3560572733e00df79800af485931c12e02a09ab962f68402917acede9b9aad8d3c0756b474603e842a5cd13fcec33d967cecd" },
                { "hsb", "156b75b66aab5a2bb2fdc4b4137e0bba07f91cf244f9515182e076fabe96747bafc9a5c4fef894fa09fcb645b66ff6ed778d601d8d0ae1e63cf5f2a319d72bb7" },
                { "hu", "724871ae472b51a4f1a1198e3c844ee0555ec73e786790dd447f5a17a22929fbc6b899147ec2dc82a094edab03ef7a35cfca67f64b5e31d547b5ed37c7faaba5" },
                { "hy-AM", "104198f294db7b7b8f2d027018e228579ac2d5d1aa0a45c6a4d4cba2650d353a0c4a20867ef540726c5c2a457e8ea4699da7fe297cf6f45d62d6857153d6ec9f" },
                { "ia", "23fca94f66b8cead3991c17d607b5bdf1bb405a8fe35448aab7bd671df18ac59698091c09e5ddebe68fb42f5a4d1e37cc7315b9b6f790d90624b705647c280aa" },
                { "id", "286a71a999b4a45c4d737684ea0a9ab5adbdf0815c511704f1b77e4bb34687eff447ca0da0d7d77dee3f4648af7568f8071ea7e3070bbc2b51b534b12dc1b1e7" },
                { "is", "c942ca8e43667be50f43fa535a03d54dfe492f54181faf2ab9de788f5c4470c435a079c1a1cf547ed0d25f67933e139789926a14efa8bc5cb5aafb23a79626e9" },
                { "it", "7f00c447195538b32c47e4c7d49211d48cfe8dd858c084389fb28f4df78f44a5c7b21d9b02da2cc8b527091827b0a3a76f21c320460dced8158b0492e263c2ba" },
                { "ja", "84fdc309a8482816fc370e93ef0662fb8eb573fddff28273a36cd3bb2959d6334f3ad1cfafcbdf4fc7b27627f2bf6567dce8c83a07d4daa9d23cd0e4fc3b4b55" },
                { "ka", "e43db835c1d8e97e31c827ab3dfca547d548804b4323fcfbdfc05da03494388a1498343428c2754fca69f7afdf34702a6a3291a7d96b779c926f772754fe63fc" },
                { "kab", "6278266eab8881b8c8810f9e2b7638588075fe7860f7a8e24b91cbe03303ef88f0f175389e095c5ba750566df16b9e5244e42235dea32d399a5d9b12f995d62f" },
                { "kk", "2a0f5757901e812b265a0199382e3c52c68ded420fc45f6d5b2994aea5d7d1f0c2c9c5ebdda937c423eb2cac1f09c2ccabe4d96e7ce786292cf4864511dd7d9e" },
                { "km", "d34bfa4145fd37b051acba101beb909a5e6c277fdde2bb8813afe591b9182aba585f6a7858d15a28534130fe9ea14d2bd8f5593aa75e5538179f0ceb0bea7760" },
                { "kn", "ec0e53b26ed8d1959c4a693aaefbba9214628f644883a324d525431f6c94995fc9a496813c8d981635bac17d461690b1d52003a28075512a20acad8150648a4d" },
                { "ko", "a5d673fa99daf7123b2b0fe89da088e4f0e327ff4487b4edac12bf4166e881aae929488658128de103b51516050c3df5467befd406f231f2b38ee1adb0b55616" },
                { "lij", "a73e562ca45905a6c15718d5386b05daeaaff59021e29abfe8c82fc5363c2994ff6cc9847fc70f43a918ad2b1a98537e60f073b13071ec01a11fd896beec466d" },
                { "lt", "567d2925ed97e5c104d31051d005b702bc79565755f8712d0406c66b6d22918a07c168aebdf930c04edd38c98d78cd63b4482a985af19d5a656d97c6c9b2a74e" },
                { "lv", "296091a6968e2ea06724d90eabd56be528f2ec366ea6c33d52109f0a9352c80a3bef946486bb427c3e458bacb408234496530025c6f73fe5b36521861ea742c2" },
                { "mk", "ba1a024b48241ef245b32d2936ef66342e5724f81c7c794a9d0d27f2de7f909392395db94514426b89eb8669a6ce352b363580084a3517cbfc5dc3fa62188745" },
                { "mr", "292af005544078188d6d879a56bd46a1aec03eb1dd7e3a54cf40020777b135d7bb092774bcfd7a7399e46bfe25f8a1a9a94fc00509ff41d5b076dc9673478475" },
                { "ms", "8b8132ec4a21554f74dc08edd4a61805b313332247d49247a81e587746e0e4af8e7a279af4d53529d2e4a3b9b29d12a233e3fb9886e3fbbecc5b703e2b951ff4" },
                { "my", "3500926383032bf2f1d7d6c6a0026f6296211bcd1738c1aa40f8057a725423d959906b452ba63c303e9deb5141344c4bcd606fd335115360ac20b5a1f58ec660" },
                { "nb-NO", "2fb7df19c94e1459739df7f2cbe748ad6fdf20f74caf623670dc7edf52291110aac6ba875f7f4617fd16c89a2140a07cfcbfa018186dce0ac49d6884115e29b3" },
                { "ne-NP", "000b12d94d6f843b1fd4ea384148bdab2f956e318ddb87bb918201c8aced58bbd4713c5a897ccfe293d093801dec7f591899414607de52f179619262197ed922" },
                { "nl", "ef36903067067e8b73ac10a50cb6f7c5e3a9004965c7d7f95b382d39d0699a955962c3091784fb5fb4d7d8864e99a9b3cf246eb0e400f31190a1044b340e3836" },
                { "nn-NO", "678706f913aac59f6ca791e90380f38d837fc450d7a517b39a48d2c76fe65c4f1cda527f128f86d675d6536df43f23f7956b0be8311e197bddacf3eb1ab6db7f" },
                { "oc", "6b4a59bc4573711e78c169cb01df5d5fbd8b2b913e0a5b3efa37d4aa3542a32e880935aa7f8bc79d72056d93832ea46271f8cdf226671b1e60f0ae68d8799e37" },
                { "pa-IN", "d005931673213922c3b59166b9c2b6c52db59a3383b1eef17e4f69bbfdccc0b0f910335be6211d34aa7c64cbcc3991c164f30090a5b0ede52c018b99555e46ca" },
                { "pl", "7ca717f6c0c480fefb48733ea65c5de15d0b8753f531d864010a5ebb9de8ed51ba2fa39e93989cc8bfab9d0e56b401355c04eb95a8c8e08212039760cbdf0fde" },
                { "pt-BR", "b008c0d527972600894dcf3ebaffab1892f37156e607b9453af3e53a485a1d36210e84a8017299a7010eda7914057320fdcc56b5c2a7d4039005e774a0085adf" },
                { "pt-PT", "a80770241a85f562df53ca009e800a455de2b4be119ebae6705c9c43e184b56f25109b54a3846b04c7cdc3342e262074e6788bf3a20f5e6ed652226081d16281" },
                { "rm", "cce52efca363bf62b2208171d896d9863abd5411f3c4b939683ca64459a2f8c5226dff3b248485e91cb1cf942ebdb3ab817467ab65aff722cd606ec6d7bd4c6e" },
                { "ro", "d62d3b871a60005ccf17a73a454bc558b2d27d044d47281957687dfb0af121a42d7ea54bc2bd54d00d7051e3c90884b624410458782636c0113a2e166e9489bc" },
                { "ru", "5bf135f235b61db8bd0f8412f4ea1396a6fa603da4b4d817f0af6c458060fc23caa1f9b3da6c942aa989091e3c32304e064d32aa9f76bf869b82c9b56d7f5c86" },
                { "sat", "97f7fb42ab822cde06ddf214464d7f1089eb293122875605df24a93e2ddd7c4a799c1448edfaf5c1b7a064cbc77fb9e4c071f535fcbe083de5f14fdec3bc83ae" },
                { "sc", "2bf6fe86517dfa14216ad8c2e3d896fb8b2c2f3ac50fb9f00dbe71ec726297dd44feea5b57e9b3c8013513494220857fb41183686dc7aee5a08cb91393392e06" },
                { "sco", "b65f48ced4fa0a4222f9ac3f8dfc88200aecf4a9bee50b3dec837477f44b64e70a247e0e45c0921ab46d4e864f781d86e2386b973a39f21e3871577c279f4618" },
                { "si", "d0760920705bf7c2d271ac30c424a741ebe607a418400a3b9550480b5f64c6ecf2d354805cce07f8e911d256ca2d6c79d2b082e076fac19b4a12fe91c172e271" },
                { "sk", "a1af4e9bad76126617ab9dbed4a37627ab06fc89eff44084c656570a2138c3d85e695da12e58b4853f533bdf8b86412b822672fdd35dfea6db84d5b962c75f3a" },
                { "sl", "115d9aa04a2a279f2bd2c7125675ce851d65a579419f70089ee704f19e702a0023d67d664c5dd331819c182348e611a313ab5431002ae7a33e8080b7894fa53b" },
                { "son", "714b2258df0f2a4e03643f4e7202623de52b8196827cc801f77e7b5c028b1f2a238d70cef8aa568ac77819ea0cb05dc909fadf1e855ee7aa3d5786c89e17fa4c" },
                { "sq", "996ec1c0d9232b9fcb36ed5e8d87c1d8a42e179db2fdf24f4995776417ce9151943f9c50b5b8015caae042adf49932dcefada82f0a75e4a024f785fda3194c97" },
                { "sr", "cb99791c64295c6103fc05776b3af784ba7d2f1b46dd46854c303a328ce85d8a7d36f4b0845f56d554ad1395d08732c75c665a2085d7a29413c04ceec0d81892" },
                { "sv-SE", "1db53a86a7940233ca16dd80622e49ce09cc8b1f4fa5bfddfee61279f3b587fae7f98ce7d187e2a26c4bb20bca6fcadabaa280188845cb683185b4f484775c75" },
                { "szl", "7070cb7eeba72860985ab28fc23a1f7712a8777a10e913d97e0a0b22c0fc3a987e983662eeece441320d57c78e259d695ed26d99579076d32f48884f5f0458eb" },
                { "ta", "6edc677dd6f00211b8ebc8e6a9ba7d56d1a35f773902a6440d8ec4b7c77b78bb6aa904434db371d59ada57291b49cf55ed98e368226975e39aedb810001c2e69" },
                { "te", "a3c83ba390840eb8940c36a9a9953a2d7053a8b903e48e34f8a2da5754fe1a8792d0740880dc306baf6be4b9d48cb0e8bc9c0ea393e5a5f236eb153bf44a69f6" },
                { "tg", "6475f528359ee9f9dd3292bdcc6fbfc53f749c8e398ff39ebf02180c515697a42cbaee7e70c8e5e51481a630162b88923924f148cba6aaddd010d56a7bcd352e" },
                { "th", "ba3cad9e732dc3498146650fc61a4df446368ecee4798dc9699b01a6126068a6cfd4e029caede854ff11116c3a9a55f7aaa261cd2f30e3d7b5e8d907c5003307" },
                { "tl", "0ac3bf1e4c7c2fa6d721a4e00085205fd242e44be6b42bb124c7da028ba4f7564839ec58264ae59c7fbadf44b4e35c112c676954c073ed8e822b708fc450e381" },
                { "tr", "c458ce91360be5c4a73e27a5997e23f05dba6423276c49d62f62f496db5f8ef10324b5cebeb2eaaf73e49dbba5a0ca6f40e816e22c18203ddb231f75f6ff125d" },
                { "trs", "a10a0e6b86dab1d4b6550d629df88a655bbccde71dc9e4cc935c551c25fc32199f7f22108caa82b1dad084b3ae0bd5811eaa7701db6574f9961743a27ed66cf9" },
                { "uk", "30c71b2c4b3e574917f695ac573dbafd507d4878226b396bac1848279d4d68b332218a96c5fd390e3dbff692f9b011698c73863f2f7a3999a083cce07abff369" },
                { "ur", "50682f5cc782e0619b675c485ff02325600c42ee29cd34b2c9c5fa114baf763f3bc8138d420b6db318df5d95ee1c6b343ad34084da382daf1b06cb3e91a3c4e3" },
                { "uz", "a8dd4471b9d9ef42ed75bea9106cd31c4c23e0ba2aba59978ad9345949d9ea20bcf1708dc2e033d9b65b358d29bd2a6121f890253d70cfef61d51b7a296f6985" },
                { "vi", "c56aa047074b8cc25bb2ae086e827ce5220495279482d0b4156361e0adfde28fe942cc0bc7fabb699b699df02b8e55d2a6ffc3b926b2aba87c80bc851a7d312b" },
                { "xh", "aed3680442ab88cca64e5ad54d769cdfed845204bcb485ec54d2ef45fc2c596d009aba9c0688a87ed488004055676719cc8accfdb8acdce626bdd8e232851d83" },
                { "zh-CN", "69e05256c3584e13f9e8895caf312702839b39725d7721f93834786ef1a86a9a013a1bc2a8b3e313e594c199722469a08fc7a7308f7905a7e2419d17bebbd4a6" },
                { "zh-TW", "c490e638011d8599374f268bd59cb0939d8891a737e356fe37a7f6a2b76c4f4efe1044d299e91526274142fc970c889313668dd6441218b42206055d8e8f6f52" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "37d895578c93fcaca56b0f501be8056fd8760609cdc9533888d1cda63d342d4159df46e767ac72b38e5eec458c3246cc1d749155da0be7813b434706fa715e39" },
                { "af", "bba2aed3f72641002a4caeb8d56672b7a1e1b11a8d8e382dbd78c313280b18a70e59aada3b3e2a2f52a09fed321b33e4eae3ef6fd4ca4faa38b51f20f77d5dbd" },
                { "an", "2b35c925cb8958d0ca754980897eba5371432d178141993efc34c4f26c0364f2df5e07a0e981112b64f895742f41d962af0ffca3389fcdb5633e053bb6456e62" },
                { "ar", "7a0d151312e4161849b369baae0c1757f7f7cb890ed8fddf5de4598e73b7fb71a6a022cd5a429d050eba053a7569a33362ac17982c33fad8fdd72a1b070e46c3" },
                { "ast", "dfb1865728457942256b21d80ef601cb9a6fc2f3bdae4bc3e498b2e98725457fae6dabe7199f1dbbb969fa6aad40550461dff245772127a20fb8e8415b9bfd98" },
                { "az", "11ab5eefda43bfd00b437ddfc68d1803487a67986d1b799e79d45cb61df9b74e9c063dfe6ac0453f9d2f3c901432f1fbf89dc9aae82ec30190a5f240a0414235" },
                { "be", "b6e0bc501e5e2338c167a8070a93d0b428fa2639134245992be95054658f92d52f4dc44969668df9d6d8b977d7893591a806c3aee5a1706efc5bea6f24963583" },
                { "bg", "7ce72a07dea8257eeefdfe58b3b2ceb2d5f773b29ac7694b9acde0dc635ef2c4ff434d477a289e5869fe72ae337f1155ba1e7e3e0d68677abe9cd21085ee427c" },
                { "bn", "63c82f57f714a7003d541cda762ed6f387c30a6e369ec092839f9b58a3706df9a8d0dd41ded8db808b48fad44dc22b5a77cec73736ec4c8ecfad254c32e8b8ea" },
                { "br", "3052f5c2649dddae3173a29154f3161527f000c4213c9bbd6c76d4ff45d71693ab43c54081e93903758386de26d15690dcf42c0c9f9265638d56dce3781f912e" },
                { "bs", "1d0aae1226c32f8289e7240beb8a349d5bc8daede268d9b1e7efdccc9564b769cc300a838365bc81352526c748a827a6214b516a4a45e126420201f8eb12942e" },
                { "ca", "09b4229a60a816024439fab911a652465efcf7ff581faff5daad7052c11803436d06cdf62bcebd190b4f66b83b41e81e30a0c0d0f51c6e2e3e63886ab59ef1c4" },
                { "cak", "e36e3ac20144e540daf7249688939de499434dc9d4965374eb3de93fca38a3676b68c37cdea63ec4eb0f20e3f2ce579eeb12a0b11b1e51ad78dfbfa880d7e985" },
                { "cs", "f870450f0e6f1ba1805e239922d505cfb8490a7d13fc65ad5f58e9470896a1d6e6a7579c6c855a434ad9089fc790a0b72d2342c344d63c33a5c57644a78384e5" },
                { "cy", "9f87a14174c8cf1412645819fc4eda01bb6e64b869d477fb1481e91b7a492a2c7453636cf06ed601cd125d0d5f6ca06d2ed86ee75132f2dfe9016285f7d6b1d2" },
                { "da", "bfe0b7a3bdb72607819e4dae81e29363dc317f7b6f6173454caaf2134a90b169f0422499c78d2a82e6a174a59a36e23d1a234a3bcc15642b1ca2ee35b24d3f76" },
                { "de", "4802c62b48cada639e783f72d5a374f00c947c2cff11a2be087e92b6d000c6823d3e39c8fe6943d70c51764f266278a1f4acc9d3a9a79a47b554c162501ae154" },
                { "dsb", "3670810c9757a8180a6596486813e14ba76ed00299113c92b455d661a7a1000795a78f0f99c00f554fbcaef485642438047179a7edbdbb6f7c14029d807a6fb7" },
                { "el", "ecf23c1c453c1cc376f3dbb00ec05db83f968c2c9d3886604fe5368fcbc55bc1ba2292a24ecc80474b99e448285f9facdd2701d4e81bb0a37b677fc9a93eba69" },
                { "en-CA", "aa5b1287e08bcc83875c5047f097ca60401d969c889667a742106689e8fb9d171e8d9bef8c42387782290723be58e4fe7f0d1e9e5bdaa1ea5c3df293234260e6" },
                { "en-GB", "c82d00c2ad476f03b20174a962b9b4f2e891ca14acee02d776f2ba26b05465da33fcb059be98d8331f999de267d81f95bde4d3e71df75a3d92547bd8e3fbe263" },
                { "en-US", "75aca5742662f3b2e193b1a1db96a8e7f94b1ebbef1914b03133ade1eb977551c55c8bbdcfff338cba53e34f1ac118fcd59f54da21b4d53032c49d8390947f6c" },
                { "eo", "035432659c4e2e32f4f2ccfab38247d7a5e15a0ed9c19dc3b5fdf85fcfb73df913d2354f139fb332482e4841ebf061924246d49d7ceb053efb5185cccedbacbd" },
                { "es-AR", "89c0e1275a90d19020e4752099206f4590df9152e26583a47e6ba4e90539bc69a1cb146a3054d03c0e2d99ac44626186ca2df71a097776308f979f244c41721e" },
                { "es-CL", "9053a29f6d137edcfe43c91a07a930511ea19ea1a102e40b0f53491ef6c21d5676114658be7e4bf0f0e8acd2e2c901e1b1e6a3d09f96bf065ef4b0fe2601fe6d" },
                { "es-ES", "16bf461befded2b731288f99d628e09261e942f7f0b253b5e69eabedd2350fbee7b395c65593ea1a562791343e900d59cc4774f835368cc3b1344a2f52e369c3" },
                { "es-MX", "439586888422b9fcf1237bde8bcab7b4660d3c69dda91ce7b2a23a598cceb2cda466656dc46df9fdf80e0b1b23fa8490104725919ccbfe5bb6b53ea691072874" },
                { "et", "5aed608e2a6d3119b0d14bf76dcad1a92d0bc21fd67e307d74ef03d2cab2e1323d555ab4e41ce56efeddd6aea170ee39a17e5c7b51bd5a2a893e8c46e07ada76" },
                { "eu", "e7bcf7d95c1b244df35ea1974c1cda54c141a08d0fde69d9830701e1a294a872f7bb65445781b214a3b596e134e318b910b86bd165f0954efa16944d8cec0525" },
                { "fa", "d3df6fa78c07d6f0ae855ce22544ea8207ea820db5d868fba584d66bc8ad56f0f6e2d99f24b7bcf4999afb1bccafe162579fa1989b95fe7b12870911b72f55a8" },
                { "ff", "ab125f6e74f63c6bf34b6cc7ef7fb149301263213a724086d5e7dbf86d86f68afaae2fc43551c374e09cf0b1447785e3a0b09f9598449993d7d60ccdab63442d" },
                { "fi", "1a8d521f5a4195da142157783ee4a5fd1b54869e60f057e18190ee83b9964241160b008fb3cd798629160e1e3418eb0c1d68ba18279a7ed789f57e6bedcd33e3" },
                { "fr", "3857cc2e0cb5f9ab4758895b5026fd2baad9b1103da83723cfa8bcc2224e1ac104b896ae8c8a1ab86edcee7e778a708c39323e02efc2c3c7621a59649a24b81c" },
                { "fur", "52944fe107e6354eed72420bba184d097769eb7dde35d1b0ff4545c42bfc6118c522ff783b25a19408a2fcbe4be59f116d399289a4c250698e2cfc6a3ce329ad" },
                { "fy-NL", "d665f9d94216a314c82065b0fa23101bbcb9daa6ef5b8865ef2e467e59c05cfdf3f90d1ef6b81a1d499f8b21009a006b66e275cc76fce012000212473d5b690c" },
                { "ga-IE", "7e1c8685a7369084b630a32ccbca93fc4f0be7a98f9858e097af195d90404475e29ad98d7edd5b7b43e8641b56820a7ce3e2f480208cd3214f2f81a5df7c74fa" },
                { "gd", "812e3c09f001988e177bcedd061ca41e5f21f2279e0af06f000170659e5afdf82fd383d541e14edbdf332f89ffb591bede74c36d2c341a9c85bc7d5b24637938" },
                { "gl", "5159eafb843505b23183f6d3658bd7fefbf51be194036c08001efd23d11c631a8a1846d00aaa96a4e518d46965ecbe291df5f607fa7793ef27cb4c406ecc741d" },
                { "gn", "469ba7536c3e78448e446dcd2eb759896fbc14eb8465bedc8f8e99775ebff1889fa2d0aab5f7ece43d70f84ca73b42b68d797a61df88a578d70daa1168acd649" },
                { "gu-IN", "e54a0b06752c884447fe83548b4cd8b34906129c466d8cc15804973be71f4e3f5ea3b10c106327a8a1dd3571ae477c164da00d2e481f830fd63db0d6e68838be" },
                { "he", "beeff2e67ed7079d94135c36be3670cc793bdd44a98e91f3884bd9122ceb14b94288900a97d93317a782cd7d55f0a1c27dbd211de4413f5c78a7b1942ae25ec9" },
                { "hi-IN", "4a0613dc131b082f90327ba0fe52c75d08632abf78a73bf99891a89a07708cc53266bd9ac1c50290ca705b9a41d385b3d2d06509f73f9fdf024f7b86a90b3bfc" },
                { "hr", "0c77e73b39fbb875ce8b5e627595d9d1da83783928088efd6d08c2b39c695d838fe06ccef440b0cc4fd233bb71f72f4cf93e44bf6c3473745372bd1dcb647841" },
                { "hsb", "12b11b57b78bb8d9dfde3f43563a1690a9fc9a1e8640d9c2ccc80ec75c2f2b3ed0aefa8395871a84ebfe6403931fffcf50eb83d5565ba0374bf8c5fef1791b96" },
                { "hu", "1be44bcd775009fc0a9ea47e03e34e3fa51fbd215413816887a2d00e9ba2100be5f6abbff6d7d451ae28c07139dbfd3f02f28f7abfc871ff56e3c10cc8df65d5" },
                { "hy-AM", "cd3f5ea0ce65db10895cfcd14f9af34d83f7420f95dbcd9804f1b72f1920964dff9ca407e990643ee7a9baf352da00d2ddf630fb2cd27e1ab717d28d53720d63" },
                { "ia", "94a5d0b529ae5a9593f490368b8afea938296fa2415b9722cd83753b7b733df393fc418781f81adec3e0ca24a85382a441ee519274ec1d5cb57c0cfe1fd2bd2c" },
                { "id", "a0f5977720a25b93e27303093e8aeb9ffc3a3fc9162085b794bad08fe2f0f90941149d8993bd9244ae5e89a4bb502abc413e02ec0db87f086485f8455eaa78fb" },
                { "is", "9af8a46ffc9376ec3503b4c00dc9c58c4864adcdddb85f73a4b8d58546327537f604ed85cda300329ef60472df6aacf4d248fe6f74420a7f740739d2d70e59e4" },
                { "it", "9ef51b32c7e9293eedb2e5170682853a3539662bbfbedabb66b1f44683a9be31bb64473f9b9d7f7a2cdf7b9add7012a30a9c4be7a54aa8db65225dcfa7b6d9f9" },
                { "ja", "a2fa94ca22bbc4a4cafaa2e00401041ae336a512fd9fa9b4a622b5e8f867165d7a7c54c27ec7c539f0542a99fea943c3831afffaf753bbdbe90c9083c57519b6" },
                { "ka", "508eb6db530561d6aaf689ee52e6b7f7fce7024ef83170c5a2e7e8021703925a23ff91348ccfdb904e4109dbc9b41cd3a179798d10e591436a83105fd40a9a37" },
                { "kab", "363e3c3c98312d01f9e7987c006fd660790164ac0e1741c22b5db4fc4390ef830775a02c2c8f293a40a13113556b5019235058b77004c057aa66982247c375f4" },
                { "kk", "231b415684196322f80ad1c1405bf618b23da9bfe657091ae98bc222dd075da6cfd34ef0eeccd8696813226a10eb201548673dab3b4ff8297337454ad77ba5ed" },
                { "km", "6c4155efff2e45d45f0591664c0972a14abd25f59478592e1c083875b93f86278abf6ef03cc02af52373e44075a957c6d440cac417780e463c04bc2393500f3b" },
                { "kn", "0560089396f7274da7aa2f4c63addb617f95ab5451f13b4be58c8cc69a9e37172da37a0665f28a683a79281d13ce8f7ef2ac47ed8e42a5675710b6e77ea770a5" },
                { "ko", "78d43a8e5cb3ce8660080aa5f47e868334ae3eb9db36ff949b8fd4ee4423b37667982394242d7b0a3e0d1a2b8c54d849f65f36f1933de990e5c775c6ec8bdd6a" },
                { "lij", "fc30eb1d8e3247b654781612c79945e4388bc15cd601affeac6fe66824fcc35cfdf81e8e60e9a14bf8fc609a626244830382fc1e84278349fa4217253b17073e" },
                { "lt", "cc87ce7b7935c26490e9553cef8bc85d2c25b2e1e12ebded05682297d20ee175f0e2f766fd7916d7cb4581447d90f320cc52ebfd6e9ab96c2589fe888a2129de" },
                { "lv", "d7dc2b8af63f4d21c3460454d23cd57772c7d56693c36dbb62a6bdf625afd5e662b667a9c6558213b8dcd8be9d017add0f53b8e06f8c97b04b7892c8fe232c91" },
                { "mk", "459faadbd232e56d9432c94e92f9d1bdce479550aae9ba9de69dafd1cc772e8472b4b83bc5a43c0ef19c10b72faed1cfe85401766a5ac553341c6e061d4c0cc3" },
                { "mr", "553f9f8ca447a3563e5c9690391445d687cf140bfdf80e0b1b2a1e9940024c0f5abb20ae364eb3a1b3d786d8f11bef941b2e0bb5ee4b266465d4b657e54346b6" },
                { "ms", "b55186bf8b387bf739575792d05be9e0d30598f48ef3d00b0d05c1ab1dfa291788505ed7cfcfc6f9fc1a5a2576472437b398dc9853989832822b0941ec004d77" },
                { "my", "baad8ec5b2e5a4899065c87825f6b14cd8e20e5b5fe1b472a6f719efdf5433d128e7d8a00d18d6fb4db9a5571d72f0fde0f6dc1cd1543542a62dff6c30dba664" },
                { "nb-NO", "67291776ba3c0b52c5c41c5ef34e8a32408b2b4de838623a0340e1d0d1512b9057997e55c864b64eff034f1a2ed13e51286602b2c05f4eb2f530f7e1f167b49e" },
                { "ne-NP", "00ab967f5ff83f5811d086d57b55ecd2248b27625b99d10929b921ef33021a2050cf8c7092a59c36f66d94ac00fc8e67f7e662faf7b9018cde409b310498275c" },
                { "nl", "7f63512fa4d236b14388cc56da366691edb96473482732bf6d74bf67b58462dbf50eeab1bfd430feba5b20e220dc744beddc379adfe7c0c42a8600a617ae0ce9" },
                { "nn-NO", "909a20cc5ac440200133af76f9aa5b4798fe71e1290d2cdc525211dea8bd4ae35e73b6de5c405dc63567d59a9bd68352c9a579611841cd4c8fecdac7db314210" },
                { "oc", "6efb33bf3e1a76eb05037451ccd863b21871c6fe6faddc8203c6a98f19bd0cc2604838fd3a354c6040d839f7c95dbea0a90ed174c8f19ef69863b126ebc1d8fb" },
                { "pa-IN", "adc403d346f172319508e6eddf14318ea07c5221a64eda50e2d9f41a0e290ec10a1cd55852c0093e9da5457b7e5dc19644a7de2f60c6c86a1c7a8e900527dcae" },
                { "pl", "15ed26a765abefb6817bd0c86d9932ed387f682a807e02545f7c8aaba900aa37f6a6c2786501be724795235254b49df4ce773683f396f26623fb3a056fa44756" },
                { "pt-BR", "6d308fb45130c366b04fd64d567981bf53c1ca063a291f3a13dbdec2636b61826a2e8b577f05d84b25f8eabfea7d84f6d90273ad2115691e5d1ac566b4700d22" },
                { "pt-PT", "55dcf2c85dd185227c5411b5f5e337379cfb2be1f5e3c0c9c0436b5909e151ac2069589586412f8693e2eea097c2091b00e7e075130c800efeb8f4a894d39848" },
                { "rm", "7a50e43960f11db4b2de40cf77fcf399775157b4137fd64ac9298799408d193e4d13d60c03f4ccdcfdc776a307dd32b5a9287479f6835ed5f1be3a367bed8ddb" },
                { "ro", "3f61c0df0d39bef60258fcb005d5f6bd7308eb64f88a09d89c0ac793f7f683f520e389f2eaeb9d93a8626ab2cd8e4943fe81b520db34812e4d505e076e27b8ac" },
                { "ru", "376216f4f8d599f8303f1c9f0fc28916bc4ad9c2c88fd24b6e0ec4482acc039d891515c219755437c3f63bd1066c6e167e1dec29e8b027d193ba27a94690868a" },
                { "sat", "c041fecace7277f86ebf04c1d42afe3b57f7930cfed5c9a9b4b2078541e2fa9c60885104907bb33ff538462ee69a7dbf785a7eb9a990f151d7ccf203ce2deaad" },
                { "sc", "726f63b40849eca3b16fdee29cd465bf335b46c365485ee7ca5cce458e130e09b2f23a0bfe7da9a4c8925c3d496322b44048ca89ef8caadc0949af669c78645f" },
                { "sco", "8ff619bd1cb99e284e9c34f46fc685f5b1a74052ca859e6fa10db21ee0e9fc6993582b4dd73a24ac0d9222332ae98221dc9168e5cc99c6c2d6b3b59b3c39d960" },
                { "si", "2f9873539611cad68b3cffa22eb781488f56ca5e7705b4675e4328f46ec47922389361dd66a932abc7c9ba4e90afd54eccd97556371b75feaf222ee3a01587ec" },
                { "sk", "f70f5f0ccbbb10ffe06a381a25d7b0a104821ec0adcd9fb5df88112f881fdaf859210da5c82119db5404e966eaf756300dc7c15d1ca269182feb9ad6387fc133" },
                { "sl", "f317b28eb7837fb1ed6f9bffbcf530f3986abbb953c6e0ce7ee349d2ed0b92df663f7f243e16e3fd8c281de98070e0fbf74258ccc344dae1c7f7eba48afb0b29" },
                { "son", "755231fbe6a3902d39fc60b9bb46169755d4f76a4f68d2bcddf47200410fec3de52ba9352191bcd14e4a3ca5fd573a6e88d1c7acca7b45a84dd2bdd1a453b656" },
                { "sq", "1742ed00e13e8666a1fb3994ac5c8e363ec78bf7fea6d4def16f252fbde7d4aa6dca19227ce75a66cbf098a91e6eb358ff93d145199587e8ba4fe223e360fabb" },
                { "sr", "c4fa93ec801141d9d4493a4314be2c7f010032b690b46a4e8f3530bb78b019b1235d5cf3a3eea647b34c470e518e648da1852b6c855a3300064701b428f35e46" },
                { "sv-SE", "03bc58c328f598131076e61242009ed0d0249e6c07f1d7b697443284547c686d3566e74276205ea96ca920289f86e6941e42ec71a3e35e8a8a5e925da0cf17c7" },
                { "szl", "9b032de612703acf67305ec3824094061c7cafc0611bb8bf83cc609933d851a9c47bf7c5a8db3e9c9295b1479090f0ac0d7ac754574493d76d8717da1f80c0de" },
                { "ta", "9503cda474b89960b0c14a10894438a1e3f714a2c44a021491f48f22e7ec68dcf04ed5a7b7359306d9574042b6585c6777f1e7902d0384b61916386f24cdb267" },
                { "te", "328a49031396667c2d84a8cea6e6f066354329d1789da01a492cf280f5521ee87d05b54d9a1adb421c0acecff3eb7997f6fc43f9f3fe31f2b2abea9be1030215" },
                { "tg", "1135c6b77f221f4101ddbf2251259590e37afd1911ffef9477f822316fab658a53627625c76b8efa4ae4b24c9c27a5bb3812fbb903f03a1c5f837f16f067c4bf" },
                { "th", "4dc0a39df2599116225ef041353f878533ad7a7dd2bd53fcf2d03b902aea77943e417744c17948786214ccef4a034691038bac3968b702c9dda4cc52f39238e7" },
                { "tl", "d632931056f5c00f4fedafc23579ffc2ced44f9fcc3e151fa9108687715152d03745c8da5ecaa3439c36cd1c4f7bd4d3f1e9d5d08cfb36f4201a77f3ad226d72" },
                { "tr", "528f17de65411f133a6ca9e525080373c5a02c2dae547f7fb4eeecedaf9655dc114157a375b52631a8ef9571cc65999d7ba46c1eb1e6559da2ea4275556fe34c" },
                { "trs", "c36a0cfb12223ddfd9a9fddaae9f3d0761d8f1bd988ab7b832fafc9fc103dc5c491c93ce5d321fa3fdaacc43e8782636fec382115b6431e25ac62cee1cddfcd1" },
                { "uk", "f9e2c1ef7cf225dd25183083d07cbfddc84e199a0d58ff3a67d8b3815c8733b3c580badc859392513c9842359067fb88e80721b56fdd3f8b7e4146cfafda3bf6" },
                { "ur", "86e29d005cfa7ac1bfd114461982bc2eb8a5de14396aae0942e95121cdea304f1ffa26140d00ab1d76c44e7e5f5a7415b570caff13cf84adce982d1507fbd253" },
                { "uz", "a31262236b381800044a32d7a3caae537aca4e97efec55233a70b0f7aabbc5bcec2fe973fc3643af81ca118ca47ecfacdcca7ae532a281eb74891f1804a7a471" },
                { "vi", "8c353261b7412539079bb580ef6b26850313ec70014554306f443fbf4a0ebfaa49398dbd9f16b77a1b6a8bc4832beaf1ed824527981e28d25e8fb2b19a3be540" },
                { "xh", "1ae74d6a1dcc8133442cc22a066468cfcb2bdbeba3dec01ba2609a847d586f92635a473661fa124222c7a686866df7682493fa2a3657acbdf1adc7bd5fe189f9" },
                { "zh-CN", "a00990718391a1f5c90d4e97af742e273f4571e827ba86bc388bc2b0d5a4d631cac7b8f302051625f8c6b83eb03fa81116e6093332a6c14ea2a993f294dee8f6" },
                { "zh-TW", "3455dd520ca644710de19f091720e38997be6c257ea101a4193c3399760730ccc76647dfcdadde2be439b70ce2f0f901b829bfcf0d2d84fc3f44d1c3860d8319" }
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
        public static string determineNewestVersion()
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
