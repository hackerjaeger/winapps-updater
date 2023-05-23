﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/113.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "faa7216ca6cc1889372f41fb80ec533948d74a49d3334d99a469f53d967574e21fce467654335b11fe8090bfde279582c19de0378f190735bd51b32797460a71" },
                { "af", "1b9e7baaf1f4351a189335a5a92b0f8744ba548a18cc04434fbc79393821d56d2a27afa9ba8aec4a8cdebf25932fa4c0744e24be553b19deaa452fbca32d23d0" },
                { "an", "8bf9327482fcadb173933829774cf995468e0b222d9d2fcaaba48f358b47882ea94b8723eb8a1e1da96d1d18f28b3f97598ded9fbae215c86eb80e434d63674e" },
                { "ar", "504b50c79fcbe25d0f9412c2c2ff73e7f033875557615a5feb44ca92a8dc76ba6c59e0a6812c1de8c7330df692a9de3a6901cf237b8c6dd13c2a348ef2c364fe" },
                { "ast", "207b00043dbdad7a6352b9f2b70d531db1e061c12c7971d0f7d483aa20aca9d7cb3705898bfdaa012d2141e1e575e245fb997b2f68cc98950e43b0c88d402523" },
                { "az", "c9169587161b2cf160d595975d2b350e4527414053a14f83d92a1ea5d8d7e7ed7d78daff41ca98cc271dd37166390af72eb1a33f2f43fdd5c500adbfe2be77a5" },
                { "be", "73ed0b0d144b54add16aa78789fab25aee025defd92603b56705c07f6793b2626a043b2a07fc3f30289c613397b1544b7e207940ab6249816812c9c0681b3904" },
                { "bg", "089bf0449b46c89f73f95d6f26417c49e90a1947904f17623371ef4c8e9525ed71b224c2d457d10d6ca01eb924228e3b4863a021ae741373f3da589507516826" },
                { "bn", "5220bec9114f45d93bb1556affb0f34952606ca8fa28ce0aaf1c285c73894b939707a3d7a1b901708ee2c84f58fe90b47024b53a8bd5735d6c7ceceb72882fec" },
                { "br", "b218a4814a8cb36ee38ea60e8d1f882bf6ab0f12a2be1c65a88810b3b8526e5480f54fbf211cf2bb3d4aa586997cb4f039d68cd14ee8db2f7d37568cf9486183" },
                { "bs", "0094c38bd75ae6f59d52a4cf2e5a5927161df8ea0b60f45d53f6319fbfdf9ad87945f6786da77a7884bf8091f51a4eb69063042c6b12197746cadcb1e654252f" },
                { "ca", "247851e9b33bd30f12d7abade096f1e63cede22c61c42e73252f44448b0fb769a0d20224e7b50a1f9b40ab9a74a1cd2f5b65788afe2a491386855f121411b0b8" },
                { "cak", "6f6335446456541497218cf6a3b51788e4a9995a76c164838283385d75ad35dd8ed218163892f328fcc07f679c7096af281251dbeedc6a4e1a7132e79e1a80e3" },
                { "cs", "2f6037624e50a0fbd7bf6e6ffc81756c9e47e3271ed8b7971073372725484b9fe93e2168256bf10c0148c9d4af5f12400e44bf17d53502e0852c26f91a0362ac" },
                { "cy", "6e7acfefbd03f9e3e3382aaf0aab7ba7ef89aae450b995a6c4df67ec5db963b8fe507f76f057d89be03e84444e9a7844ed0205d2528d79600e2f186d3cb3a778" },
                { "da", "e1cea99b839cae4f407b721b352248f3788ec2064ea979cb9ff14fe0be331f7886241351b468e40f7b953fcc1695c6cbcb1316167caeefa9df1ab016602988c1" },
                { "de", "f27de36f9cbf3123d5aace5d38792ad737569d6e43c67f0ccc0878804131c104cf3f3f961b05f9481bceb95bc9761ac0b50251738984214e8093ce2f7c1ee649" },
                { "dsb", "ae5019a5d9f6faebe49eb6d4011c87c0ea8568ddd219c1b910db1bacf2bb55681ed92145f39d814445e70e6e98037e62246f6d48e88b3654941402c1156e6b2f" },
                { "el", "60c4b92de71b3ec740c94d01736c2e5205286559b2af9223556c05323cf6e8d90a32d35cbacbe85749c4d0cd38a1b29fc33eb065ba5f48bd8baaba431e202ed1" },
                { "en-CA", "a57543b097448c36007f74ba276b645e9c5c2e9278d768a613d0876e2341ce6544cad6d9979ed2ae253dbaf7618d797736923c6556f7c4d33751558cc96f4ad5" },
                { "en-GB", "1a596134a64db9cf2a312edbf48618b1f7375254c3f59527d0b76339b65a6ff56816877b356692adedd3f9eb2a070358f9db17aedc326af9ab43de18d1252d4a" },
                { "en-US", "4bb23d99119f1707b904cb5fbc3094b3cefe6c8d283476863ba5f824afe529703d1d0cc3fabacd9c69db1efc69e460eec5d7e2b36de2a09bc561fa4f85fdc647" },
                { "eo", "be6367bb83591b4d6ab4a28e4522092cd1be50ba15569318c1425c70daac1700bed697517f3b89b835a4134c62b0218b138ed570fa6ba339c21db5fe680e73c8" },
                { "es-AR", "5360ecf37556c168f009bc56befff6c8da473119bc67a94ed0ab8b732c3c2df060b75b624a97a33af01b9a36fe8085bbaf51cf4984fe0bf70d3f2d89e0edb1d3" },
                { "es-CL", "fe35e503954f0e6e3597321e11ef21de541799f6cecf010fff81b2300c406751adb18fa82a6c04ef1039f071f7c1279f2fc1ed7418273558e5ea1b06069e82c0" },
                { "es-ES", "7fb827e3e23ff497964ef0bc8e5111c31cb63a7a3c5cc1bb5d54e031970a7a631c8e4b8390afcd6000ccf34d5e372fa4f32278740d6f7c16c30bd17ea72252c5" },
                { "es-MX", "7566f0c0530b9105c0adf61ac8ed507e1d017233488bbe01bdc49199c87821838f96fcbaef95870928a7b1fb6c7c53a193daca9bf3f4e800c61263fd8c220c5d" },
                { "et", "0667b3c9e2be2f9629fc198a111c4b1e7991580b3db975bbafc4afa1bbfdaf7712bac691358d6f902788be39ef0cc96ae91e284e793809701124dd2ba6ef63b0" },
                { "eu", "05f3a65bb9f446f1b03af9fb193549250cd3cb0cb5be4bb9d93afcf303353156ef99a5ed88572fb70e7c966d07f46f0229b6a466ca62a6fd915df9c73aec0802" },
                { "fa", "3501d6a1305b6bfbcf4e08ae3de5dbd50f9610a8da61c76682f9278307801234ce9025abca97a5b90c00ae217e2603bb4d6677b44628bdb970e5129adbbc0df1" },
                { "ff", "8bee4d67be29fa0ba7afd56188642d9cd87b8e9b18742652011ba85952d05ba1a88a7816375a8fc20ee5413f0125722f02d038db73bd91a32d692fd54623c21b" },
                { "fi", "fc47f9699237e9c05d41c1522132d81c7943e82103ad3ef02ac7f75a1717ad924d746c8a654c2b7cb933747ac87b157850cd6def693aa9b336a7456dc7c6f624" },
                { "fr", "bd7db2eeaf196c4c0f52981e3af42f7fa98248dc407fe8863075dd13005ab2d9ecfdc61dec513f34947353f3673d5d5e404e165a358c063e822b7ab2da6bab43" },
                { "fur", "a0e5b5ffed74b7563c8183542f5114503c64e2af8a06e8bf43a6dc65b34195a5a56553a1d80b09374634622773dbbcb94a0f1c71b223746c8e2284e5d1687d11" },
                { "fy-NL", "6d602e1dfac043484fe6c160060efb74dc4a8a71fc9bbf0ac63c52ff3d910dba65d12254847fb9dc6b9159b30098e8036aa4ab7b1b197e668237e9d12a5493e3" },
                { "ga-IE", "cb4ec4725761f26eecf8b01b6ab72ab32ea9ee9dc1405d22b0fd0d30f6f01d84f73afe9714fbfc6a75bb301dbecf27f040ba8564010c954e458e0249eb2b797d" },
                { "gd", "ad3674dc396ca250580c3b2a9727c6bd612979acda3706a4eb32599b9b1a40ee7007cf7051f9e75ab74cdf7034986bb092ab45d3452c352ff9b7087cb704e847" },
                { "gl", "22007506444ec1546735b4a07d47fefc6db6767402a08529085de7be9d4e837bcb12ccc453f94f8f54d6a9344d6a9796c9c0827db9339d44f80e40c017f384ca" },
                { "gn", "421212a1030fb7912bffe56cc81f3752313db48f2466f1ec55af2053b21a1ae45bbf069fb2db65ef14c29d16e80eb7bae463a06a604a9015908926326e1201b3" },
                { "gu-IN", "351b00dfaaae22661dfe26963fb389c26be42fb90c8730f9cf36a83f2607bf214bfed669f208a2efa72b333ae7c959338e66c5f12fb3ca4e759c20e406a46f44" },
                { "he", "c5ec9572364c96960e2f4fb1481975163d4173be15a59566bd78c836c8aea67dfe716f7ce9ab1f2f019e5cc6a8908de8aadce2be56d4b200b794670c7ac405f5" },
                { "hi-IN", "ff9abaad6c9d42ea5be124a0776543bc8022ac646f5acb31bd143d77cbbbf21d14b8c91858c59a821ede28530e9d1b7f8ccbeb5a343be4c2021e3ddae06f9518" },
                { "hr", "bce4cd5143657166c4bb78da55316552bde633e5da73a7c8fe65f262671c1da77a5cc7dec92eaf0a9daf5eccee49b1de8ce94737c14aca2f362da7854049923e" },
                { "hsb", "1baa587ac4b7ee015529a176c6f73cd235cf2b406c1661aaacab016bdfd99b1cff6a8b8d5fa7dea1ffc1f3bb8d3f2d766a5f9159cedd206b73c670cdaf73dbdf" },
                { "hu", "3a8d5555af5200dad9a15416503cd3551ffa48fdd11764b9d11763872ca01ebfc46f2e9f18756141f25e589ba74fd0cb4d40daee78d90117a959a1d51b8171f6" },
                { "hy-AM", "6d93d2ef2d9f4ac5809cbd6f29d492b3a1a19c5319e795937dc2f434feeb2ea85098285dae682959196024c391889941961ac0dfe5bb0f0f0c0f44f13b10566a" },
                { "ia", "e957b8113b0642d80cb91b2e182cc744af404dbc7be435584424320b993d336f1a1f7e87698ba016203142f0a183893f0a56f29817c5d974ee6d7012c31af923" },
                { "id", "1e462c4c561229f9887df7477a42001d765afd62cb0b071c6d5a15afe3c0b44b1c3e28b7b8be752704d1911477062cce6ed2bd5275c0519d720d3f60ae6b9f4d" },
                { "is", "0e526b2aead10d65cc6744cd0abadc3d549b18f7747ca27e57266c76c8dd38b335d6eccff460c1b4d29934e036300c938460c4a29fff6ea0b0273b0354f1b1c0" },
                { "it", "9861f28fff0f428307e34d16e69959fe7169b0bd6ec8873f8f4cd41a9058d48b9e692ebb253777aee1acba7c31f4973f3a465e719fcccfb22e42078387d60165" },
                { "ja", "ae3c037f5dc0d7a9ebbf706794c65f609c8df7be2cf7fa021602360eefb3a0169069a575456ab9e8120a61be29c638efa0d86d8d07cca40261e49c1c93c90dc6" },
                { "ka", "7e9fc210d7e74eb0591513a41dcf343d28663c3b52efff1a294590916900f49d64892909100925982c205785df1a4077d6e96116e5d9f2297fb8ae2b3c9d66e9" },
                { "kab", "48d22f676358eab2c89c21088235ebae857bf070894a78b44d551692f02f819adf6d585f11af55812952cd314030713b5d94fdbd32857643ba8f619f603a3eff" },
                { "kk", "fd63c9fa6959ad33d9460b04523ae69282d075e8d93ac171fb8658f3490d85cd16bb491c17187a89677ae905ab356871f5a4a9d175125a32c77e0c7461348369" },
                { "km", "a6ce402dda7247dc5f4f0b347251d8148781abcbaccb9b5703bf3012377d20971fdbadfccc05e1f991a7580ce7bf5f611a089141ee2ff45287d2023aa6194ee9" },
                { "kn", "ffb2b6b3875c54677deacb3df87a135e2068a4d87d0458c4474435d97c6abe13a13f471d47ba3f924a30a204dc7d9fb1d73f3cdc7424f65e7f73d3d548123e9b" },
                { "ko", "f6e1618674b8935e5e01425b24e2dadbadc23ea5d5ad3a0a8e0a7ab615d885887bf809b273b76be8ca6fa16d0b79b444c2c5ac96d1bc7c8bf771cf31b47f25fa" },
                { "lij", "36f38eb71ca175dab2fd93621188be4859fd6b050e5f52e394cb083564e1e21e63ce0afe23b11389790d442acdde8fa632c32bdf340a1879f2123ffbbe3cb6f5" },
                { "lt", "b5793c1921a488f1f6d34bc84deeefda028d2e293ce18741c48cf6ecac8ccf06319c1713f6da4f7b8aa6bda5717925f948aa88188cf1b80a556c96718128559f" },
                { "lv", "df1aff42e57a7ba1b7e2f229335564abe84a8a0bad144b695b1f0c19fe3e5f773267db756e1c83b98fad07243fa950a61402636ca8ea36291c0eea2479547c7f" },
                { "mk", "f80133beecd42f916733f1da45ae754f4475707ef531c75bc213dd21e1bca48e7343fc9a8388dd8aaf679ee9f4cdc31172ecf9638293b27bce1482e734652937" },
                { "mr", "bfe6793a02349ae8adb5fb2b04fe8e9f729d616be25105e7b4a533c67dbfa01655700a3086e66565ebdb4c92783d3ac12543e2712a0b6a482aff120570c953cf" },
                { "ms", "0eb7c6ca8e919428cfc352d85cc2bcd2204b5b079cb539b5ca5c16c3d8dcf3428264fcd16bd00296e80b9ed9157955472ea63ea0bad18d8c191276047373b946" },
                { "my", "fd55b9c900a79eb6679086bd2800aa6ec830ed9e812b19fa141c6db6438db9892c12699fc1980f767dee116d1061be045a4963753abee68813062a22c9e7e389" },
                { "nb-NO", "2cf0c46b20128976c0892cc3a09017f5d7529c7ca0bdd8340b37292ae9d05edd7d95179278f93031782fda46ba7f91ebe0ca5153f6af31233d066e862b2586db" },
                { "ne-NP", "84ad19b447ae8d0fe1f656ce0a0bb381ecaae66fad11600a36178896766d6d2110b768d1f824007f6b845fb0ad6ac0def56d461235d8e759f38d714ca531a7df" },
                { "nl", "727682c60608409af2aada9566903f99ed17d3e080bcf9d7fed8461952d1aeea4e080169c229c02850f55636bc51f49e21bc80a4e502187edbcac74527394150" },
                { "nn-NO", "fc1ed1bd044fabdac4a22f8990c411f21881f5a6658e7f90aa03104ae13b6c147f60fb4a21e2a478a90edfb4397654e1fd815ff0d3e8ab05919cdb4803edabcc" },
                { "oc", "c704f9fb31c470a00c23ac9c687608c90d8e0b5eb90eaadcc7f2773030584508941c036a661428ca94f23396f663b5d2ad1dc70161fd1aca6e58aab462cbc12f" },
                { "pa-IN", "f3338daf89b777710c3983b83ea0a85d70026a6364334c2c7c6977a6d10da3ec5205d00e220adbbefc527bd63f231113d3f487f36b67be47aa3489a4174d1ef3" },
                { "pl", "dc6c95b75b084cb44cc32abfd9572f2fb87408fdebe98e79cbf6bf706d46a9924809d80459eac6cdefcb6b2244b9a0783a37fa39cec7a680b94e8afb731c0598" },
                { "pt-BR", "c09b6cbab750b3725c00e3313aef9425a64082c392fded08217c34991dd4d9aa88a2707fdbbd6bd8c01f661a9d7c39ea9ccde3b39579a1a65976516345197b13" },
                { "pt-PT", "b68ba605596bc6de93c176bb06b7343eb079269ce74f3c3eaef8e9f1e34e782d8855affb201b721ede1c253e9c167c14f657d63af8e725d372c7b6f2a01176a6" },
                { "rm", "5b7b242f3cb71ec6bfc4ce623c1f2f423703625df85290204269e0e918eae1ed45d476246893b895034151aa4a6b7f1df911d823c050e8e49e1c37f28c7a1ced" },
                { "ro", "9efc167bd45b3b7039882cc34a34fae97214fc3d0a3f6c8492caf5816c8e533500c3b30a483022e40616b319cb01d150496af1f2b5af7c4c1d043c323e2420da" },
                { "ru", "73fb1f4ba161aad0a086a641b96e9cae1020d63d5bb98e0c213e44b3cd33677da31d9a2cc00941332173bd8df9b4acc97649c8493d55fef926ffe8e663b89dca" },
                { "sc", "bf127ec74af32787bcf9da1c244cb6fbe0e378efe1ac4b5adaa9d43d8fd5dc42a29da3d42abb8763c792c465a18f24a77cd84c785d90dc939b8f559be166ab0b" },
                { "sco", "a564d04cf0af4b169589fd51d83233a06dde03dc7371b0c5b4737e123e81383757a64acae34d929a4ff189eaeba53bf0f2b8e26458575ea45d8700e1ccd3f6d2" },
                { "si", "3bf1643e28eaf86ef2a454943675d8ecebb39a08ad3d3f8ae17514896f3fa866e7fd46f22f200d6ba0ec7a89a062edc399df890a79c25f5816d254d899958c93" },
                { "sk", "2c15f276a7abeab8bab1b8107c2d8b9e7499d7a0036dd4fee277a03df0780eee46e9b78c5bca710c26c2e9d16d954311f21d14a4f45d0aafeadbe95aaab0f3cf" },
                { "sl", "c0639e449c135a993e22ebf62511a4f521d13154a0a1d428a222eeaddc6a51fa6f3edf9e43ee2e3e7baa07041aa143121647ea2cb46e7b54eb84dc4470f62a28" },
                { "son", "ab031516c2c415eeae8721e4cfc6f4d12da60a49bdd32c64a351026fc6d4539c87da37239625c630a1178337896e1d528bafd78e362568dbb86f457eb84f73fc" },
                { "sq", "09eb12633f89e9bc5b5a17db46823ef80d889fc992ce5213b47bb05fe6fc2e6eb3ab604376f4aeb8eb457e14d6933f7a376f400aa417837841337f56360d9acf" },
                { "sr", "60d08f5a6d625747302dbf483797d8fb843778ea9022db913ea4543d3373fa3c00fc4f0e9b1ba9ff590742dbf67df611dc7883a3884f8d645c054aae8daee3e0" },
                { "sv-SE", "3bf233037bcb5b8b212809cca276882f67815d0c459c766ef4ea24a75dd06376a583c7f0201af501055d3961dfc595d7131b41eaddc4a48257ff692c1d7d0b47" },
                { "szl", "8def935ffbdfdde9ad56383ff89d83c2c9a79db9e3c9d2a949a3de8919db4771a7082607fd323ba9b22be458257f34c05c8ac15952b11e075eff12046b92c68f" },
                { "ta", "917df984c12d8b3c570e6111eab9b829d14dd22ef72130f103888848076f1e19f6d4321c51fd8844b042bbf0dd44833d4c109fcf06afbac3b846b3f3e8e348d7" },
                { "te", "f3b9de1a0d9d5922a91d5f6328cadc381c5b18ef686fb8bd326b1193c2074904d9efb9a9c4dee8c0b0b607d9098670e1943780a36757ce012cc5ebe930789cd5" },
                { "tg", "e7e2ed0929c610529428329801a3e9c313ba25eebb0ed940a5e5abe26984a0a997a252d93897273239581046e0b46ab1c4df91b9140ccaaf4963a1ceed99a902" },
                { "th", "b56a0f78738e08e7e3f3264deb50e1ab003d8e000a7643c1a2be11eca86bb8dbd08373b3f742f5d3cef2e31a3d4aed3dfaf3e0bbecc405196f6667cce5ed8e2a" },
                { "tl", "af7a13300e85c48920d6897f3a0e5abcd66d63d67d2e027c828f0e2686b78495c4667ebd04956d50b3a34197ee7df5886536211f667864ff9c51796a0108e87a" },
                { "tr", "b649521806f654cce677be431771bb3f955876b47f782cb66a5010f7257e4c9311011812115e21c986561a4ddf841cea3f4bde9c46bd8e8ad21cc684450cb41c" },
                { "trs", "bb4fb18747e6b5943bf9bd14b0de814deb70e3fdacec910e002132e2200fbff349f32e5f382b648a1a4b5207f8931b673a74404dba80b9c36728a48932bd77e7" },
                { "uk", "b920c1e600ef0794402b2c4e31b4f548f4d6a4f54469b07678f552bbc19be6bac91b834e5fde2a82b3d4857e9d164e2c2913bba4740953d6e75d9385a12a5104" },
                { "ur", "85929e96ab71544fc64760c2e60b04333a7c191230b4c437da43362b2039d9af1cb767b8d31521e681a4509fc0ac0677456061136132d4c4cf4c290c85a94e27" },
                { "uz", "e0aae6eb8db2289bf04568ebfc1e4ff0dea408cbd9ff2ae771cc6eb5ccc9774ec2c3288b8496453a775b986792d9d05fe345c1e0c77b5e4dbc268cccc68e24f9" },
                { "vi", "00ed39dbe2174fa406dc357f792cbadacf5dfdc3d380a808fd79739c6bc51824e0f22eac86aca2d40fff3829e0f47189d92f0f69d47f4ad5030d4f45f65bbbd1" },
                { "xh", "2ca284d7e24b2adacbba1f5468cc581c6278c928d74513760006e3bd7229c9d62487e6aeff06300730b3bdb6c0cefedaca3dfc57c8f2a06255bc23e0f3e39eba" },
                { "zh-CN", "d339d3bbc8686d6c5dec2165ee58cdfa5df8f7b73991e8f1713ef490e6c953d6f0dc39df0f7f9b207787c0d01d2e93c404be6cdee6b4e6f120f5ee4a8c875a96" },
                { "zh-TW", "0c9d38c224cd5a2aaa6f1bde121d0404150fc683cb2c6e765cac6459f87f8d7aa2f6af44d1d81cf0a7333cd938d881309479a247e13395666a8760cf50544044" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/113.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "74d79e009c36a3d5dcaa7cd5e23d1ed6d9977104dc0a6775251e4b2641730456c483cec3fca5a19b22a16ca163f93ac2eddbe41e9ab26d9c1837c468cef0bd9b" },
                { "af", "d86ffc0dc278735e5c0f79501fb91ded2de8c0312f93a8c7719e93e204e59c643bf4667fe1ad8372b4ef3b0dcb5535f9bbf4a3c44692bf281bfa0bf49a987842" },
                { "an", "5cb2b92566364025311ded7f760a6224684f3b0b158456b5b6188f69bce51292c9435108cc108f255a5887f82bb7f8b52ad0e6bc38844eb9e2eeec7d9100b7da" },
                { "ar", "b9220a54c157a81334fbd78d9de7e316672f336989ee13ba81aa9334d0f32113ce60f17c2674d3a11b3865585c7e022edc90d8a44e3a62420a1ab63243b536b0" },
                { "ast", "cfc5545d9838877cbd3d609663672218e103bb97ae64e53091b5def54c7c166a9135fb0efe9fd1c8a870e05e47ff4878be6a48d045362c894f6158ba2cf79cce" },
                { "az", "b805331c9c9648112af611773e98a9f553289cbd409f487d3319ba810e7dcc9b51a39988909bbfdc74ac03792af1179e90a9af74896bef7457aedc8a35d8268d" },
                { "be", "f3af1967acf46ed0a6d945e064690d5882be6e9ec67d7b4799c4b9ac2648f3b9b8197a0205fcde957229215e129539da4b06391de89aa07a74bc530a7edeb841" },
                { "bg", "4efb45de3a9330e4f3d5e7323bc8f6c92e673ea8ebc5fb572b7954a6ce2d4e54d0cadc94884a161e80826d73b346e1c424883f78f69954d3e70fb04ee8d37c33" },
                { "bn", "a7881b74c3c205a6839fe39b3277bef853bac6ec7bd4ad573cfddee8d47b1ed933673822c4d96fc60e42b3b2961ac331222309184d708624045de2425a744153" },
                { "br", "41dab775ec20cbb4dd6fb9e1df12b5763986ef89c682da64006ec036b4b21ffc9b078deb59fe9df083c77fa6fc32afd8d54f2fbe937bf0148d8aea6ace5e4a65" },
                { "bs", "db43cea68f6e71b5692a3175e4d2096aa66f320a098ea9bd2cd8f68e35ee9e6f597aa4004cfe2b336c07bd425c70bc922059e004983f290984c2f1fb19c5aeec" },
                { "ca", "58418b6575627793bca5712d97df413a6d5c1217b8d3b92bc2fab904b521e8ccf1790a34136e26223112adaef545b46dce18f4b6b2e23c4025ffd36195dc5b4d" },
                { "cak", "a87ef774272f1b335dde883654e9917d3cf23f40604d966629ee825c4c0b224f7b332784ec27241e1c0814f0bdc923874b818fc3b5a8b872c644a988a27716b3" },
                { "cs", "9a17674d2aa8d0911d9365ec55279095a695afd36cb00c30a5a52fc0011c1cc6c135f9ea61dafa34efddb2476425d57f1dbf84280dd7ddacd38e1c7d8688bd2f" },
                { "cy", "a5510587cc691e8acbcc67fccb4a8ccdc7b93010c6e8db202d979a50371e5c4536b78e1f893af44f42630a2a332db012ea52e347e45a9d6055b331398987f516" },
                { "da", "c4a7ff8524f1b4ed2725d5ad51e8f29766c8defb6b1eef1e6d4dfc92f461d4d3168b09989797caafe129ef604d9d6c2b01e06c6eca90e51055f0fb0fe4f89b6f" },
                { "de", "0186618e1ca898a69ba41a85131f69e8f0bddf8cf1be6787baae8616826d044c206411bd1df03cc6ca5b9fc0a511871fe2845cfe269d106e2274880ea0dec9ac" },
                { "dsb", "628c263055dabf66ee053b13d8c64c9a2496110dab6a6b7c4858b027c7ff60acf99a59ed0301a91ff2bf041560bb70c27141adec475587a6efb903dc4b9ee7a8" },
                { "el", "1e4e6f669e56d47c614700601d92cd398c087e267e7b605714b0764ab41121db1b217bbf83e75c713199de7fc61dfed0c8cef549409cbb31339c7ea4b44e266a" },
                { "en-CA", "2b90edbec0a92703ae6a2a3738e5b0787cd3b6dfd4cedb6868ceebe7abc871ba51dc66d19a37f40d9e7481547dfaab8ac148aea16e2aef481f2753ed082cd1cf" },
                { "en-GB", "779129e8911cc9f177d65d38e6d6d4ca98552bd31f354b82858db9b49972f74a2630552a29877ce4f85bddfc5d3b61601d2199285193f1ff0fbafb219315e2a6" },
                { "en-US", "b7baec5589c965b5a6e2a2081d2998af566ad509c824ee711a92aaa13217c2a78c9b5b84b1c123bb48bffdf0e915e28108c845e08958974ea9d052928063d558" },
                { "eo", "ea293f3b3e08ea6636a0849fb4b79191ceeb9eb7d71447ea395a0589c617637559f40e01edb57171be8669efffab474e06db3cc1b7031067f5da48bfdf26c692" },
                { "es-AR", "ebf06ab334611e301c95288f129648e34a9b7721c470a7011bf0c9de469be80e21b23bf48a7d7f9ad150ff1fd6048fefb46595b9f96bb096c6345667a8d013be" },
                { "es-CL", "b5b11412dbc20dff3548c8bbe73c9fdb5e431ecc59c61915efa14df2805c7a7781194ebb23608651c817e7a20daa6d30de5456ab0b9014d9c60895022fa6c91c" },
                { "es-ES", "ee2f8e9855b93187483cff56ff2c13534430d5117c7ebab7357e79d85e548c8379d5ef24e1f20fb3de289538c93440d1bcf53887d2fb40fe0283f6e8b0e72059" },
                { "es-MX", "8eac2f470a75c9067e2a08b940a5bcef03bcf605dae6f5795fd7b3d28f136eba5639f562e469a8f02948dd143caaeef950d12e8b44337e2e5a51e21f91ecd6f6" },
                { "et", "c2bcf8f3bad24cd5757667884bc6ef5483c04ee6d3aadc2369440e433ea93b70982ae67d3231893fc2401f3a1025c64debdcee5abe138cb9090e74f42f79a7d3" },
                { "eu", "a70e1b82e5ae6e90b20dea143acee1eea3e1fe51cba809e3b57b5d6ee042bcdd7136c8b5e041b107f12373789f83ee5656bd081c2ebecfe0c39a655bcc495795" },
                { "fa", "857bc24b8767b8d7647c972fa6055d35c9cbff8a614d35e2ab6ed3f2bb0b98f57ef0f7f75a6235692d00d08313f5ec8cd178d5d0c49eb26bdb0b49cf02044427" },
                { "ff", "ab87fb2d3791fecf1624a44b100b78636305ba34627c4a9c6dec60763ad970b27973c29810074fd33f30a61d810e561a58f28c85def14dd453a044f9c63672b5" },
                { "fi", "8133c3a14c26220f986631aa69f5c8f727f41253d87577c8795af0391adeb6b888066d2b6455988a9cb5e3c256573428c66d2fc29d3439ea5fafec40fc9587e3" },
                { "fr", "ed376db1e3411b01c7ad40d87ac3cbcb58c587959ce18347e279b8dd66a06f6eb232d6d603c76a7868b09f0c35258bcbf398c087585e42a9e3ac6fcd800977c1" },
                { "fur", "2e96eab653b93c853010c252e0a8bdd9fd42c9aa09e191f9ac9355ff07b1c3c736679720ec37e7ad1ea10feceefc0fdf4b9da842f50623ad5da20465c54a7b2f" },
                { "fy-NL", "e036b9f891e58e3d240f19268294998c42fb0b0804a6421255949d8ccac23eed8d392eaf7eda53c4fd4b597d8a02f5cf1495d9b306137e9d323be443044e32fa" },
                { "ga-IE", "6031af19bcac1ac6b37e13dd79ad1d84bc614cefdc2f03eebf2c8dcd4c58c5690dc55f329576a11feb0b4199fa7fbac789473917dcdbc098cf76fa848f3b9c0f" },
                { "gd", "b26a144aa8a3d0e23d1dc43ab186b6bde3f38ca0fc02d1beb5d935792e0915b49d80e243ec73319159cd8350e75df5fe3bbadba04d565b3230c87384fb24c404" },
                { "gl", "77ec6a9af8b9e78afc17157c8f6d24859e6c44bf4036991291edaac9b08b46877c73ad6b31fcf6ebb96dbbd61ffca848b240726dc6fdcdcbd1be8e0a8a408eb0" },
                { "gn", "15e2aaf1ab0cbb4251df54da1e25e7c2d3fa18d6726ecc8ee06bd5f5f11e33549a9b831fb41bc71c7735e225deed5067a765db30dae3fb5019681fd1d18af681" },
                { "gu-IN", "ab13b2f9889e96282d1e7de3e8c27fba0e7b769766047df8d940440408974492b1d4c123e8ff9ce06594d26033dac8d5fc7f9f3d6d82874a86fca55a52ae68f3" },
                { "he", "58d24c35bf6d96857524753648bebc70b747945af5a6f12470296f2ffc7f125b28d5cda40a964c1234f2f54e7ac90fd58f7c7163997ba23ced465b5db1de3a9b" },
                { "hi-IN", "6d41023613f2a133077907e21509162161b6b7845bae072c4a260d9099aeb7ca794f1b717f16fb07a8032c68dee6ea2ebe8563529321cdeda5f2decf645f9d3b" },
                { "hr", "64c057e1ff421ef232a5da1daab7e63844d97a722957d6c89f584e85d4b5a765c6f0e69b93458bd55140ff9960389411b5e3088a109a4d2cd5cf245b06dc444b" },
                { "hsb", "22c22e6c65d8c3d67d5063811e695447c188bc9dd578fc213dbcc48803e50d0f2ded870b509f54fde50f7527d4016d64a293aaf48a289da54f8f5245c4b2c885" },
                { "hu", "88f6dbc0b0ec4de9e24437f4ab8f3eefa309f19068a9ae948a2a581939b12197340904ab592f546f5d5c4168c262fc1827c4befc23ffff670f24e37c6327cc08" },
                { "hy-AM", "5978b324f5d5530dab08bc8c37bddc67b3e5442dbd4ee443d8922f6282262b8afdc8ae91a7d4d7afae5dec82ecd959007df04064396d0695508a9e3cbbb4fb01" },
                { "ia", "ba15d54328278f4694a3906a5e97514ed5a96775efb6f3bd2fbccd5adb1131faa0ce7fea6c7c8dfe717103a86c18cd55472bbd2a0c4837206c104aee2bfdb352" },
                { "id", "878e4d313c83b799d2bc895e6cd0c3ccf4f094752366d433edf027d059ee1ee234af677267a1df3eddecad6ec85a9d7078749590629ece579496c42348ceafde" },
                { "is", "1a52414fdad1c80a98ffc81af27ba8b7ef18f95e89b8e002f2178cc232a070784b49c608a9aefa334d92b021b18717308fe20f3f1c99bead6644e252160161fc" },
                { "it", "3f3ada39283cee711e0c4529a7d4927c94c73fec2ecb5fc2e36d16e9ba038331f02b6c6008f2b6caccf6cf55980e54fe0fc71b63f10002821f642b117a052310" },
                { "ja", "e5100d1e5cac8239b23cf0ab0a1dc4e890f291e019ad80d7a9313ca03689b8769cb6afb0e5ae595a87aa4ee6a79f726eac430de9ea8586d874fdc2cd485b7366" },
                { "ka", "d1da61415b885db1db4ea889fb17cada19208b6c48689a2ed3d7aa3f228a6a40b161b138915672504c31af9cc5da5af891f543c7a26b9ae2d30e3b7f4ec4bd3a" },
                { "kab", "06cd1416995a4273cf966052c5862d5144e63a14b64535c46dfa6d49cb59a01c65b29b6b1aabae4fcbf4e1bb14b602e9393af6db18a6b3c267f5810498c22b17" },
                { "kk", "f5a2326690ccfc3456ac7089fa5fdb4acb3233bcbe9c5b182ff31cc9a852d45ae85d5e30d0d1b626ba23535fe60ac4ed8473a77d8d6c8a49ab779a28f08b9e62" },
                { "km", "b8e9d78935093e33bd6b179769e008d15416eedc3311cf211b70fedee81db3a4af8aee1946c3f373fe803908ec8c12349847ccaabf3db0982bce24f8c02d5bff" },
                { "kn", "58e3e2006fba1937c3edf8d64b9295acc3e26179cbad77338b66a9b7f235535e6056a9d14f8075cd17c5f453a155c35f55a219853877d8016dd88f1560eedf4b" },
                { "ko", "2b100303744de8acafc6dc8a9141485849e109618822618afd1b9a5389c029f8ed9f448b70747f246b8ef17959d53868c81281d0df745864848092947a00aaf8" },
                { "lij", "b6463e0387152524457175c14714cb77583265faa475661b86e80e27f3a0bdede9027152bb72b30feca3bc2878075b75c8dc6f374c1921d022e98393aea0fc1f" },
                { "lt", "49e313febe1c60be3d2053c0236a32587a8f0924b6a4dd7bbc6994654353d75ea5403bc0ac850250bf346b12c69b7cd0f6d6e2858ba6d063709f03f8f0870e1f" },
                { "lv", "c5126f86b1b90b4ac811e2f2d4c3c1fffdb7ba37899ce9658c153233d43d8b40eaa81d9e17bc3f2d1defef993c20c45e17690c11a5662120ed96206717e67e62" },
                { "mk", "f011f2b5847006adce76fbe89298f552aa43b41cd65a73654b20846689605c4eb20205be58eddff3a0242ad88bf3fd4bf8dd9c68278a3319059c2d334d8b3c6a" },
                { "mr", "1748bb5a6b3c3997b939be176d049307f5c99b0a90f9b2b87aa1db4c0f390da854e4da460f26cf96512db824f1fef3ee4ce06db2963ea7538114029e7e6595c7" },
                { "ms", "2c6af2a5a4fab34d21f295108cd6b47a98b65820e657f1cafb61ae25cf58cf4396d515c112a211a51e3bea3980ab5edc065a5836cb6677f9e17b484737043854" },
                { "my", "b8ca296b6b526ed88f541ad923875d612f1e69b71739e5241e9c43f977f642193f325f354d59290ee6f32b3a8c6972d46c336a180ab474081c5401bed8f74a56" },
                { "nb-NO", "b00be3594fe42987fa8302946d471641faf4d6920af4705556e300d47a3b18127005f6e036de09b200d6b86cc4b388d541ad89d03a679c2a6b13edb723b509c1" },
                { "ne-NP", "93631ca5e66ce52955e7471eb0eeffede42f19f093c04b3471c13c0d11efa8bf298237f27525cce22eace0dd519cd7aa2ebca131e3fc1bb848cbced4af1b0c78" },
                { "nl", "0a0b813e4249a986e3690227dd66d5eabe69be696dedaf1a04d790e893e6cb0b3e71338f231ff065c7efa325eb5a333130102d21e2093c462dffb28b6a9af2c9" },
                { "nn-NO", "dd2bffa0be6c7acf2d82049c9bf1b2745b6d7def124184f35eb49b7ef1dd54f756aa463406ca70a4e651511a6b36c907f30b4dc972a9d63a11f18cbff1b4881b" },
                { "oc", "b52bef20bfb3f6ab9072104869314d54c3e04be48c7bc6e146e34b15090ea30e737b31bba4d323a04ed1c3f0212743979868d5fcd9a8e311400d5c9748fa1e00" },
                { "pa-IN", "9ab85167b11d52f97e17fa9664151c33574f6eed12dac90252e84806070b1db65dca0525975bb2c26105fc76a7f50cc4f188bc069c853aea2b6f438534b78c94" },
                { "pl", "2240e5cc95490c73a319977c3599ad52417dd2189634fc1d230b53738576f207923d33c21232476c971123e7396b8c7eacadb27ad718dec40e0d7a7dcb73cc89" },
                { "pt-BR", "5c2dd191ab757df924e69d50a842bda5545c58ee28d5aea8c0ad2a79709e4af4f7da6295fac546acb7302ec8a8ef24a81f37c48bf22be6448bcddbade4150c4b" },
                { "pt-PT", "ac871a8eeb591563a125b9da289e409b7adc58121a3b5a60125f80d4a9ea04b0696491d32f9091b06e7587ee74393c9362bc57d0d439d9d081a48167bd4b343e" },
                { "rm", "30db1fc558c0389482343826c1648b63deda72fdbdafe093c9b7ec8db2e8f8a0c1fa05186703b600c663c78cef7a7b52159af2d6259cd7c3c2018a157a641a79" },
                { "ro", "fe9825ad2c2d33c536780a272483c2501a72e4228bbf4a7f57ad8e9721feb4178d9c2b032bc194e4546a17cc5531da819cbbf20d03dd6b66ad0211513e0036e6" },
                { "ru", "3ab179dcc0abfcacd6dcfe84e441256457221f7b0700aa4722bb1398afa145710893f774bd0a494e1ef158197314b9128f40b6c1391b5576664f26ba511d3be1" },
                { "sc", "567cee9b16b31c2b21ec6a5aeadb4b725b95d762edc95f6978d139d065565090b7c1fe96f62c81b741a015da636a3da619ca3e374df6460607d4eee3d7366f29" },
                { "sco", "3559d8b731d0cb4464043fb2712a5553d9ad682f5520afbeb43a53694ed30436b21fcc11dc6d6cc96d11ddd197f5ca1f34a0c39a98b73d3ba3a4a43e0c4e40e6" },
                { "si", "897b7fe146bd8a36d774c85c9e644404556a2bcc58ee129ff85e322acd8d07a9e126f157faa2f3dcf5803fe7c164ca74a43b90c59606cf36e05891456e4aa42b" },
                { "sk", "487af6ca89caf6c1138bf017f88d6ec9c6fced7e9d0c4c5d932004d8ce1e306bcb6786473dd6b68599a0bcf0dfafc8f9e73cc1b68fa881103db5319313ced51e" },
                { "sl", "aa70e7e0e59743b917f774d6e949a2cb8aa2b09459a78292b6f6d65853dd4c2b9fcf7fc094d76d5501b4bc5e11ab642041318b65434cad5edf25e17460df2139" },
                { "son", "50d4c4b4d930c526e856c1b0f69df6f29b2b82a539b8d17c4f4703d215e1e7407a4f3bcd10653ee76647c5fe9618e7774ca1b826968e3e6f538ea05afe5e90fb" },
                { "sq", "fe1c831609900bb7839d2f6c4ee04c639f1f9fb3deeb37d8b6e7464d210c48cfc76efe7446f938bfabe6064dca120eca8af2ccbea71ae31679e34b77b85b8c1d" },
                { "sr", "17962697bd8ed8ad1deeaba66791aee4c362dfd838003ef95a5337fca86fcbbb5c6ed17e36007d936719e2e927de4a5cceeb03f4bc75bffed7e9c915d76339c9" },
                { "sv-SE", "357a49eb77ccad39ea04de7981a4455ed2c4d4537e7488ea3c321da0d7dc57870e00539f345ad2da93bba86c6fef2d60c305e07a9db039157346395aeccc0d3f" },
                { "szl", "99ba685de205877ea4d08dea39f84c2b8c6ca0ebeff645f6b9d4f4e879d9f0d773d7f5d64e65c1103f560439e7c21ed4f2b86e0c664362d24399435329561663" },
                { "ta", "6052dae670ab9778f2985e41c09d1fa96743b03690693c1f473f505d824c66c309c9178d093c09268de22c835ef0026a26eb9e3e1e47c258d5b41bebef6609f9" },
                { "te", "fcbf2b87e0dbd90ffc191f474dbb27d2ebbe97312e968e647e5e62659a5b6721067fb613f12fce2d4c2941f1e98cc3b5278c1a830b9842c636b4e93029eab8de" },
                { "tg", "25729d2ade676a84fd641e2e63b2e79e68e7af4f5fe1852be10b4681d5042e84350f1cec107ec36a71afb4a6275d532cf0fca1ca8a93b4a81c52c2bf6cd2d351" },
                { "th", "3474fd888ac43c2d407e92465102c30234af5edccef286b310bfade26f07506de68f8359cace553ceb80c82476c45e71cb741c160e2244f5ec221c3070626273" },
                { "tl", "85a3b0081a5bceb37ea71d11b1c14b87cfc339e4ef6ea4140946aee929e57024d550c87131848391c3f2e15e1d9e268f6a35b89716e15d102f02e80d516fc3d3" },
                { "tr", "183a06fb35066a2668f909d1bccea937da536d7bbd30ed03865b395a42ed0b2243621ed8c48e03f3ede0ead9f748073cf7d0714dfa10d7c69786cf13c60322d3" },
                { "trs", "c9ad2f65db790b72fdd2656235d2edb88cc157d85f82ee69db930fd0c173f152a88f85b9a418c49042037bbf759ec36866dbca666afd0dbd2be8fbbb4e86d079" },
                { "uk", "ba90e30080f6120dfbb868330ec83f90d46499f5566d5076ff08db011dc59bd192a1d55a514d3a00107c350a883d85a35ace8acc640fcf66c365aa6f26e47ab2" },
                { "ur", "499a1d3730103d45c53b75b1a636a2bfa55752dcced17a952ddfe3abfaac2ad80e1fce9e26757064b136521359ae6a58fad48ca93ffe2df0b2306a4d7c33e20a" },
                { "uz", "3d6fb5f185b95ede0fdb27ee29b945dc1607e3b80d35ac4c7fd9a34f4ef338cf6b7c20883858b51078688adcf99f34ff78331a774d5933419cb703141cb4fbf9" },
                { "vi", "f67ecc73b722adba06eabf6d192420ea33d689526aec5419eaa04b48874c6d6be86748dd2a1ded21fddc0669dde29e51c206cda28cac31bf39bd6da08f3a3476" },
                { "xh", "72a5c9c2874d3a6e93e773f112535443d984ca2d5bbe2537af30379a376176860ab7dc685251a311fb457bc09fbc20398d15298ddc06136fa73887049cd74c88" },
                { "zh-CN", "399cad4fe23970403606a0686e89cf9f4ec31bbe9e01cf5fe0145f9fc79b9afc883865699bdada927775541cf67e8fc2a1e53915e7b523c551d3f66c4140297d" },
                { "zh-TW", "69c24b709902ce49d53118378bf755c456b44746781b21fd12e3990890a17fd784f19a375bf61333f73b737b7e28e4e37359a37250b1e09117bf83b82a190e9f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "113.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
