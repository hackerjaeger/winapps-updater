﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/135.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "63d031984b017c1fdaa8c7b862720bb5669d208991761205bd3be887165baac3cf105d22f5caaed9e9b6e027400287c805f02fda207b6d270a194b6c33d280fe" },
                { "af", "867b926ed363fa1dc778e3ebf7debdc16350f0ab4664f05a002d74f8eca8c480ac01b24059896eace57e24f387d4c594ed86b6356a0ec46f484383bb2cc43c4f" },
                { "an", "be657f4ff353bfa275df6a8105ae20da0abf4ac72e995757c9c1df0518ee170baf0f0a54a6e8db5c221151c3311a796fd32775fb600d7b4f6578599bde87cb59" },
                { "ar", "6cae59f293e1ea8b831d6d30dc6748f7795fe5d72c53342c284a41512a3c682abd353fbf2685e01ee5c9cde3cf5a47b5f8db0dda80812c136b08a3799ac683ba" },
                { "ast", "e934fe283118ea4c81fa83c03a70aa90377d287484268f95735ebed147025f59827b415de16dca1622e46bd2a8838365a2efd9bf8681d7d59f75c506b6cb63fb" },
                { "az", "9d8db36192e124b8eee1082f1049fbeb0de6477a5d74fa84c0307ef82d7618181f1bb8637125661c99e654955b4de2c43ffffc951a5879154c8fbd3cb7ae463a" },
                { "be", "5e942cfe41ad8f647b71aa9cd09ef4cb1b3cbb338f816886fe34f797c4db74406e1b76f933b7f39dacaf8ef0b289189863de21223b65299b025e89616a294c09" },
                { "bg", "f06846937d7d942d4c44c33d65eb4df9e028fa0da19e35d7186332b421cd9e6c4a07c2fa22a7f7fdd110f7c3042da3ef40cce1100fd45d483a3e5c8900a3b0ab" },
                { "bn", "eede1d7e53d22759df929398edf40e22562d399da28e248ee8dd9f4a6d348d21b575d8d104f6223a1c2d68c8cd80a40c1656be726a8f8032b205c0c336a2b253" },
                { "br", "53d95c3796d018b79a38e8b872a1bc8e29b2bf17073230c09ced441f8e1817374b9736e44fc04185813c763e7c949d00b3aee27fa12fb8c40b380df9b0794695" },
                { "bs", "381754bf7449566175aaffbe255faefacbdaaffb62d91096d53337dc43d12b2c9752aaf3caa4a1203b922ca8c718b53dbc0bd62eda0f0641f3ac42196e5269fd" },
                { "ca", "2aa7a16d0ddaad3ebcc7fb5c97e4f44b3eff1c428b12c16b9d88da790c58ff266abcdddcdc3967af2462f426336ead8bd4ad019c5adae2428f0485353e8c4bb4" },
                { "cak", "1229adf5abd11fa994ad72d4a80cce10379bce362a0b59c3773f38f8198caf620ec88d38f738f1ef847a30ae9ab2e426ee01db8e254fe1ba41120f743a083463" },
                { "cs", "b6fd9ca51bec0f5d1df210e84020d62724e4d83284e7b812c062170bd6be0b226d2b3e9a7ef23f945b04b539ad2111aa2f75fa7c688697efe208981ba5d9bc64" },
                { "cy", "2de29f5e8e09fcde7d9d6d1df4761cf7aa94f9ce279ab755a639ec000ec25fd72f063cf9130ab5510c4ce1304978df84c4e63fb10cc21b441a621b77593a4f04" },
                { "da", "2bd58edac40be01d42098564046f90b9ebc143507d295e01e034f1d45ad64c41e2df49102521c316945ee434a02f30dcb346ebbb1f65f601719caf8bb079f38c" },
                { "de", "dc6006bf0a640f06ee2bc8f883da6a59db630482f7f4cc2fca27499c94a92a361778f83d1728c8110e983e11b0f0bd2f47373e72cedf181ab9f54ba7ea20baa3" },
                { "dsb", "41bbc5f43091b622a0619494e3334f08feccc1a4c84eb778119385b0464dda787be6df4f34a24d1616ab5aa90d88c5744e46aaa6931e4f72b8eb2c1c703f89d8" },
                { "el", "7fab54cbd7a8b145cadbdaaab18dcf0723682574ce260f23f198f6a50a88f02a77a0da9bceac4d03bb04463d5522bf5facf9d744473d450ba921de704867331b" },
                { "en-CA", "ca067dd0ab63ee1f989f206879f2cf4d606395546b3dd942be0be32d8e358064aa1c16747fe0e67035e402c910502b05c91942a1b3fff5126654701a1a60f549" },
                { "en-GB", "9e9212090fa0c927b50d9bd99ebceb0b8c0bffc70f0f925ad3ecba2fab544a561996d1814cd6baa98315f76b464d0348ead0caef89e3ff93f80452c0e6278d49" },
                { "en-US", "1f06523ef0a0e0464a16a4dfda1b638360f67b9e6551dc16bc3b0027e3fb4969b30676605bf2008aeeb385dc2a0fd496dd806f877ba927225dbd718cabc0e125" },
                { "eo", "aad8285e8e1eb947edfa4a737deaba4b37c9ea465e94e868141cc462b920e36a01f1c071f326229dfa777bbe9322e97ef8d88d4362385f4a0aa9258b36173c6c" },
                { "es-AR", "19070415d7b4ee8c4e2e7b8a4772f489d11129fa40fe9a6cb886f9cfc755c313d97ab4f4a9aa3ab01e1e3ba53daa1f37303584ef8d00c756e61a2e9e51ce4a13" },
                { "es-CL", "b20da4eb5f37368fdc79ae96ea5e7c5d451f309208fdec89aac0da32619e0d1ea926e359dae0341501b5aa8df3f8609909c147efaa54090a60749fd023162298" },
                { "es-ES", "e4b20831d00eca1a3bfa183a8eb2686b83356706a37c293354fc340a916bc486fa8ed49021e33fc3490d6f22c6db1560f83d82091f279c393db6d5e14776b012" },
                { "es-MX", "c48a89e59b2274e1b8a6cc7d10bd1aebdf31ccf1e75ca8f7735fabd71f732e9264b89c52f79ae879237848c070b21d96dff80072b5e2da6f10526f67f2feb6e3" },
                { "et", "ab6a102d71be0db3af4168b5cde74552479a621aeefa78505d8df19f7b08b2e1e9f9bb3fd7beac8a1eeaea248d799a6da0e8e6fcb4efcfa8bae3de88f5b1ab89" },
                { "eu", "cdd1c482b68d1c10f3a19ac8de6c4ae545fca9e06ee6067348457d6085898b38566ff2b42ddd98cac068723e077224c4872b607607b35b92c0ce723f85a2879e" },
                { "fa", "c8ef70d57a6c418c2a6e2edb6ca46e82ed32538e0562d1741bcac0da5302a13aa638dbb14c4c3e1b8085e163c9850c861ca4f878c0d4c528d5c0d926e7d7b421" },
                { "ff", "7d759b223af84757edb600fae5853051e298538677c27b11b2ca4d5100d484087a81b79a6fa6212d2644e54cc5445c5b30c0001b14265342f2a0ca5f0f181c0d" },
                { "fi", "86c1a14bf26861d86f3d20873a691d9635998a10ef83229a0bab79382df0c74d49f2dcd89e792c011043d3354c579d6340e38bc427f2aa021be73ba980c130b4" },
                { "fr", "81301b4fcda53d61f6a78d8f625143cc1ca1a2caad59b92d1eab44066496b064e2cbfbf2f20dd47430891eb23d6588ba5c2b65c1c99c190ad4ea9b5710079cfd" },
                { "fur", "5770d518c060af41eb98a1209211d61e4e197aff648899afc29763a473cd95ec377862a42eb180e36d8a630013136ef34429ae7248366e419429b078f3cda0f4" },
                { "fy-NL", "9b1eb23a8ecf5a918d33c378ad2d401e620364d7fc0fc1fd0dbc732157636784df22bc471d8ba738dd26343b95131927eeb001360c7881220c882b4950937da6" },
                { "ga-IE", "6fd14f845d45c606458e8e9fe226b3104cd4f109cb4a935eb1566c800c90adae5cf19b5c916a598076a5dccb6454f3ece20d6491bb2816df4842c1a20647698d" },
                { "gd", "d96ef7e0eb07980616ffedbd1d6555ae5ce21bf0f554b2119bf412ac30baf476a82430601d79c7ae2d86d933714733ffb0f1b3b57ac69a42e9ded1f16fce07a3" },
                { "gl", "c1f468b5306fba90749b6bbe815431c0b46fe44516ebe1c3f2c2b9820045347e20f52f3d4ebb8e1e00284af802444717516947e157e194578633badacb26c178" },
                { "gn", "646979bb29b9fdc35d1a3e444ff7d36f5846f764bdfc6546bffd7b90c6543006560c0ac767cdf730aaeb72a2bc42dfee141e56ca93b3bb78bb47d91ce77c56f9" },
                { "gu-IN", "f37f713a01faa77f151556b7b1791fa7ce5a98a5d6404cfbdae277a7264f6ee4ccd94aa11eba41a7bf1caa5ce9faf4b9280aa2766e8f3c8d55b76efda4609733" },
                { "he", "9633f39587ec51d9771d484f4be1c4067e9645c4667ff003e0c3f45b01f8a7ef3cf54a4da53ea2cbc8c3ea2677182130a262463daa2dec390aebc28d8679c18a" },
                { "hi-IN", "f23ceb3c21d45a3fa66038028762835f24735093bfe0bc708fde3000107f8863ede7e612b17c4689ec8eb6773ef15126ebdc268f74028e1efcf5ed156caf72b5" },
                { "hr", "151fce0b51a38d258f36cd0aa1adee6d911ac2781a48b6593cd0c21135c61b031dd77e694a3ecba791372da9574fa83b954f7337fb81c645b42ffbfdc35f6453" },
                { "hsb", "3dca04f8d303df79a296d41904bdf7958476f8ad519c64b3b05194e3a03b387aec7498863c1286b8669db9cb9c19a2c07e6a370f298d059509089a0b4ade9891" },
                { "hu", "2e8037e5f40f69711a20efda02514b44501946e4c71ed62f08dcd0c8020af6d7e095bf7f92cfe18bff14cd2e7d0a880f179eb4150aab372a3c7cb3e4d66f457f" },
                { "hy-AM", "30d826f9488c26a2cbdee0794c33279eb91dac92deb6fb23a7559a3d643f66bd47a7f45b4e8da48677190341efad8e99a2af38fa651d4192946d295edf6461e4" },
                { "ia", "1e70e63aa9adf5bbefe14023cb6915218cac02380d9f92edf3a3321350547d245cab1f2d962e753688d1658ac50d382e5cf4c6c9ffa77aa95ccc22c9a2918540" },
                { "id", "f17c3d2f7ecdf35b339d86216b607896c7d9aaecf7bf45093d59a9625d1f40a240351f4aa90096305db08eea853eabdc9c4951d606f97751f963e30a3d0eb8b1" },
                { "is", "77328c3edc11e7ebf4dea50ff9f96cf895b009a6ffad6b640f89808f00fb26e9646d2a52cb861e9530cf4d4560d3d336edb1227a5219496b77afbda46e6ba848" },
                { "it", "881da613da3f4603ffc7fdad6e0443aa4b538dee6f05520ab85947ebe44a4d2a4bbcb34c497bb5921979e1e3344a09e6386b07d2566daba529b371932207711b" },
                { "ja", "67fe318c5abc297bff6e3838b992f552cdb715c06423f67fb77658169b7c74c0eee1f38c1ce14d143a3e35c07bc4af083edae5a5c7053a5b4c46845a53f2e932" },
                { "ka", "a5d026cfdca7a755176d3ee2e3f589bc0e1b705e5b84b218de1dedf170442a8aed6f1d59caa684f118b466f5e02db2446d9a25204f88e1373aec56515ea0545e" },
                { "kab", "b957bc25844e4c6c04c9997c3fcaae2d99fa56ca20908188d339e8469e60ce1f0c858951ef394efa2e8f9e6e155486e796ea4ac4561740f0b38902d14e7d67e5" },
                { "kk", "1507bb3594572a87bf2a2638490eb591123b3697973b0ff2f1ef30dee68c0de27dc1c597876bae28abd7ae81259affd2ea464a395b8632b8acda41f18cf0f5d8" },
                { "km", "d3444054e7e0bd7c53930992421d34132ee009355c4ca3343339d2be1b32ccbe186c9cbd37f141bfeff6485582d6ecfabc5b8a7c08e5b22172c3f998d62922ea" },
                { "kn", "06f54951fa3e2a42b06862908177de359214a1bb9f2a9c811d2fb31deb8657f236ca64346f7fb0f64ac682b9d67c050b7233a303c6966770542945ff93b4297f" },
                { "ko", "0753ede6566cacad0f783ce36bc51017fa1ea971d7fb285a015d25c16993dc3989519a32fe8dae12d5d351da84002f61c9b1ccdaec7b93bea9ca4bd09242fadd" },
                { "lij", "93062afc3703b68cfd665aec1a7202c7c3b829e3cfa380c1737ace92f00e38ed55ea98e723e1f66d5c68768942a39df4050d8a7d35886903929942de1cda24d1" },
                { "lt", "e86579675281d9e38174203f51c14ff0945d0a8d8f4c431bcd69cc73f3ef5496868eab85bf9f26aa781ecfc887318713ab2296f0973c7538a08abcfae0b6a234" },
                { "lv", "54a8bc8151ec04ba75266c4f22947f45bd3e92c3a615dbc6a9a70986f99f4dd68041911c933e4ad03ebcb9b96021110302ea3192fdd4c3a338d5aec8694d7b9f" },
                { "mk", "cfd104cf10648ceafb99e63562671163c4a727dbc610a1a85753816b7433ef53ed26d5500444e505a94dcc05b227d5a3e8f421e64242643c449418414845203c" },
                { "mr", "e2f8be7b782a73fb88e315cf096b0bb3be7148c5fe114e420d1d6f586e47c611d1adde76a8566718b3be549cacd02eb301545cb720c31e63987cdff89787d381" },
                { "ms", "4f9722887aaeb38ccff1452e117b3aac3477958141ce16651e776d14bc2d12d2bfc68e1137ef94541d3757501b054176a1904a413138287513e293d8f2c1219e" },
                { "my", "a78924738a86b445cbd6852364c2e2f236d95dbe36f6898d72f8842ba99fe6e61cbdb4889ed9aad8dc821fc89f51fe334529ed082f2310192fa7a71979d71807" },
                { "nb-NO", "fed8f177aba1173106c5dfef881878653711e31a16e01ebea77b31f20796d4006f3e014e32e70b100aa039cb3a63bb3540a09eb6a64a826b0c9c77f9cbccf792" },
                { "ne-NP", "a97be0cdc0f602b271f1a40d78d05ecc62897620313ffe6a462d154c9d7cc00fcaf2fa65a2b1d8276909e14a0bf725c3c3474d4bd65e6a9e69a3c22fffa63456" },
                { "nl", "37b08d07f31d0a9bfeee6f4eb1e4a725fd4e4e7dabbc3037d9ea9ef93ca9e5a164415e8c169ff75ea8b9412dbf31a6009228d267f62ef28ae3fb8559442f1a3e" },
                { "nn-NO", "9c835acdca53f9ede3695a9ae146917bccce6327bdbb21a4c2b2367c55fcb888198251a04217c05425b020168e69c7099d3c2dbafa596593ca77c9ff64bd2deb" },
                { "oc", "589c385a952d5ddea7202e425f453043c51e8f5edcae9619909da5418af005dd4c648d9dd8955c56746bfb803ce8006666153f28b9832a430f1fb05e01736011" },
                { "pa-IN", "cec48a4d0e085cca9fd3bf5b6a0e2c4b13caf94db10df4bba956572458e8420ad93e65b033d07a42179882dab8523b9f39032356aa0ad66c9344c3cfcaed664a" },
                { "pl", "3c432825b8415ecd1676d728407acbd2ab99a06e2c2d1ccf74505bf45f791975a01c8b384443c68888f8556a749c04177df81abdcbea312b46cdeaf11afa1d67" },
                { "pt-BR", "7038cfb9cc54364dcd6db61298cdafbb4ee7508dd5f8c66fb93842a14b3884defe23ef78bd22efdb68184fb439e513e5c1e262f0b98928875e7d27720e12aecb" },
                { "pt-PT", "9aaec0a5cee4e5bed0cbaf4ad0fc0c4e2ea7928327a19a0074f029f0c7d349dd01171f2eab8d5214840707a9c4025f2c83c9790a226bde3d37ece75bd0f77915" },
                { "rm", "b99a36cc9d49fdd11c2a1766d46881ea89502513c10ff6523d9dc2f9f1a04f709fd0f9b3f4ae46eede6db5dd1e1833c94507a4a9dc4beb71f798e54290cad6e3" },
                { "ro", "53a4b515b6fbcaefbe5359b1d8547c8f2fecb1761b8f197cf8f1892281dd5f5ea35fa92386cb6c43d92f24c2fe72ed86c470eeec7b861c709d14a0892a1508b3" },
                { "ru", "25c2cb0921b587d1063ce09aa9f4923a113c5cdeefaca1f3d1d0109a08ad72156e4e8d15867809877e319111cb4fd206ec7eb227c72be0da53d7697187aaf3b4" },
                { "sat", "2e6b5b0a5bfcfef7be896f693250511954d1a3aa3218d744490a79a3271fdfe49b33befe1a9f8ca50ad8c664f71bf675d58f5f002172bf9e25997a2de6dd726a" },
                { "sc", "7ac508b8fe65ceb97a30272bafe661eb0afd6c162f7740e2dbd12ac7d98f9bc50edefd7ac79febcb2d39f04240b087879fcc49f77b0eeeaa848f2418d5a82885" },
                { "sco", "8f7fcc1d4168a64fa4cfc68df421594a9ba9df1e27c9a3b8d3a8835cab809978ae5afb6108d659f87f677d00d3d5be13d256daf64fee33ea5a3b0b1a3e9b3c89" },
                { "si", "3f9e7066a4cf5944cfc47f4cf73f25c7a508b5f5dbcd8326a3d43cf9740e57e854aa9945d97bdbaa772f165d0adbcb4d10e57075c4b8f88aeeac0bbf31036d67" },
                { "sk", "b81b77488fc9def7dbcec539713d26082ea33591c16094bad1c444e979ca4ae539d66afd13762a38017a8a83bbd4b1c2bae2eb05dc1a3d45e568a79d907a3c6d" },
                { "skr", "e88d4d459a87ff0b9181ce112a2a6609a79ce90b3ae835f102e656a613e72b0fbf8bd5a111f5f409387bd00b415e3d57e9e7ef30806f00ee5c4aa1330f702776" },
                { "sl", "99f2d5712068352ee7103ff0482c1dfc555b7776bb4ca1e6a5e8a1662e95596b8fe3adfe75f0c5b313b2c0d986e6f6497aacd4bdd18633ac22c41e9d8ef5b2dc" },
                { "son", "199badcd8513c4c68a909969183bcf96c9fc6b155e63b9e0fcd4c1a176484a2d9239d1c75fc1e1a116afd0b6dfcb02febaa47134d82b8f24752c05c893856369" },
                { "sq", "edbca39ce8fb8cd9f08d26c182706292a1d0381a94cf3813b6a7b0f89179cda936ea597f25bd30dcdce6d6a99eea921caf8e40406e722b4fdcd975a54db9bce6" },
                { "sr", "a58e8af78400ad33f1e448d3f5a90668ba1f1a4c1a13f7c247966d89271cdd34423884b919ba3f63ccf301dccf73b6260a878762994afd4fdfeed78491cd856e" },
                { "sv-SE", "1dd61cbf35b9057c5d982265564d8145a9efc223d7fbbd9b3bcf682d8c39bc81fe98153bad2f860447bc1f2506a0ca015bccaa4166eaa0cccb83286096b6bf4f" },
                { "szl", "a2931510302aaeb84859ad11f8fc3971368eb5a53e02c8421f638d1832750d357401375f9ebef5e50ecdfdd334439ca01db89ac572c8b51084617fee7d5e3cc0" },
                { "ta", "7bf57b4f91fe8fa5ab276f08602c8fd1939adaf99b079bf80fb7c34fe5a5abf764e48571bc155284458f8b97ee9788caff5005ea45e17176c063d85f5fb2e217" },
                { "te", "69c05f4e30b317ac1eff4700565a47dd14a2ce9afb4c268b4472090745f8f30cdbbac9cc67335f3d56753451e92e8dabb8100dcd7acb07ccfc6b75c898e7dfe6" },
                { "tg", "f371f36b867b9c64f0240de23d54f196dd3851610f727c04dbc079d8b60780c4c788960204e8a81025bf2ce4f72e221ddd46c2f8d6522198f8e14439b5f40761" },
                { "th", "0a20c6ab09535be202a7cfb028fd7a3e15a34668fc38a3feee307e2b6cde2d363323f1efdaa253eefa21d9b9da4d3dae6c2d3dbc2dfe88a928701ed993f07933" },
                { "tl", "c93c7e32cd35a648d1406aa16bb882c2e0e751f6a4de1252551fe09bad7ff0959cff850850c58a1d8b45bfba22451df67647a6c9db75fe9716ea51708c4dcfc4" },
                { "tr", "03c1deefd451df1b859348c2077a79b828b417cefb013e5c6931aa09215b94b88a91aff787f7cf26c4f9a81978e8162d70b42a15b2390dc7b993d463297fadc8" },
                { "trs", "d9f6c79864383633b909e7dc856ee5903af1899e62db58c31d26f7902295fd4a0091469ab803c663432c4deb62e0606553bbdd170d207137ebd6e3e0c42ccdde" },
                { "uk", "e5d246ef855b8134df1641c2172f8af5ee9dad2db368f1603e9d925285d7bc5d7fe8cc48d5351f4c0c4a5e22d4a3683c281ee7758b9423e5beb13a62ced0fe19" },
                { "ur", "18f9bf6c11ffc1f56a6ea6eb80ea8254e4db054a8699e1210ce74892402ac90a1f2fe97b5b863ed0f7a50dd73687d765d67a1a55da8f9a23794554424ca031f8" },
                { "uz", "8cb875439b3c51f3b770a234c807f23928c9d4dd14c7b0e6576395bbe0137969b50e4739ed10e5efd0fd4f2b13b57c7ecad621e82ff1ab7c650a2cd46b751eab" },
                { "vi", "8aebfda67671b81240007445d5776601a41216a27e40fb817338fe8766f0f02854f327c0e5d36a211edd4dac235c02bbd33c61ae33f3f2ca1140bf0499ef0c3b" },
                { "xh", "509685c4c41fe0b67013870a0897beeb5730330535041b9a3fdfcd14ea60578ac39f4b71aa49d65aea90ac020f0b3d9600588e3ad1313b803655911b2b4a796b" },
                { "zh-CN", "a72c5ba93941861f65db0309da06f51cd6a7aa7d1ddeeb0c08fe97d269734441ba183fed348163780e91454fb29b3403126a1f97ca72c42a0bbb6d025326d645" },
                { "zh-TW", "8bad0ab6e4e369690f4d6b0b39023cd4fabbcf636ca3deb56458c98e5db9654a35ac43228fe8ce5fc370b5d9c672deb32b65a2a545bcdd3cc182d745d73a9321" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/135.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "1a6a1da8d0a7144d6c4b494e3ee62bf8c66fee839ea3abb034374b0f3ab7dd15b44ab7196874ba29bad6b49c0c24f045d27c55bfa4387aa3a807402a0cdc60fb" },
                { "af", "d8997783eccaff7a85301d81453ee322db14c77efafcaa841d9463853ed51ed94c700940dc88282177856b2c84f095b6e803d753bc05e3a1c279e185d53ea2a4" },
                { "an", "a2937070425fae1ec36e503464d5fbf8ea9733ff3d0f12152b640bdbd52bd9cdc4bd2d4c9a7d7a8dd2c2e8dc73d1bda08d54c2332a9d54f7d372b48f2ed55f8a" },
                { "ar", "3d30860072620011bba82257fca2c8aa83f65d1c11057d48a6dbf8741f31c7525293fc35e51ba1e439f5c2603ab0c6f9ed1e38123933e540fc7f78c3791283a4" },
                { "ast", "f433b4480c06565391993d6b91ecbce9a464ed610f8f4873c3cea0afdf708b2c9f37d9f6164fb538c07186914180405216080e12b44c260c8062e5c895581d13" },
                { "az", "89878ae75fc6d76ed1ae232c79a52af5649a2465e128e8f9b7023da7d8d4eacec69ee67869fa117e5968aa917008eec2f98368a640eb9126a9e1f77cd3ebf77f" },
                { "be", "f389bcb61833f7db17457933eb3e2c283ce537567ef8ee92258c399692cf1ec403b9a7f2d6225e603f7eeeeb44cc4b7f61ac2bc18b4e8bb4c39644be4cf8142d" },
                { "bg", "b545407bc6b7e62921f93740a04002ed06e1a6c4bef00c0efbfa279aa19645086dec35989b4b3c0f88e93f54d572462ea88df7d97170122602cc2a5291b65754" },
                { "bn", "c20325edbee1b1610f7daf2d0f336ea59e56d76cb197b386ba797e9781d15b5418cbd24f7aef3ecbcdd9c39800c02c8b5fba5e04cac809f407540b7db041b5b7" },
                { "br", "b581fd9dfffc5feaa2e0aca45fdfaef63d09ac006cce1b1fb94134420403408d98f649f43d00dba8a2b163d435d0cb3d5048324e7861a88901a3db52d4a003f5" },
                { "bs", "2439bf1cd3a1f687f433ee1bf6154df11d1dc9111ef922fbfad5003130ba795038479f418e89b928f39454ac67f09cf1927999d3269b83506817c83e427b5c57" },
                { "ca", "c98115b7d145fbf7d475f2084e769e47a5dc1a5b58d14af9b63363454143a6668ef2344d7013dbca947b7157350ee62ecd16208c930f8cb6272e6bb6c9bfbbd6" },
                { "cak", "351483f2daaaa10749dfff884000ab91d2b502471cc374ef9a4be1699da5ce1d937c10ad087814495d14ed583ba0112d8d7f87d33d0a50936a522d13791ca0d2" },
                { "cs", "d42bba36496d0ee9d1ad88603fe2de88e3c907c0eacf4a08a7d70e0f6bc18a6ba4a5c49a67bf0eafe34faadebf7d1bd7c58f8befc7f62d1134d62fdc7f81ce75" },
                { "cy", "667c18020792b41ea94f3266f03cddec2018e012ebef8a66f10dfcb59b388d4774d2dadbe95111039afea7e334fe74e48a72718c4bf84bc490d0f0e5f0774abd" },
                { "da", "67f32b339caa55d2617bf986ed39ccd82279734502565551c7160137b9f0d80d19314b87b3f9fecf160b8c7a5f1341cdc689d110513ce66dc901330c1f107911" },
                { "de", "36e4083b720fb22df5b568b1aa4ba07a4e5fdea589c23e4023fae6d0a26e3bc9abbd7b8d5bd18e61d8ebd72114968f2d55475aac9d20cf0369d70ae84bcf57f2" },
                { "dsb", "362756edd5b21ee8bde45caa8e8f5cdfa6c845fe3356b117c63cd87780bd30b52e1f28498a11e6f13ad9da29337c563d2d2769e730e634b1115d8cc7a3b8dabe" },
                { "el", "dcab5961db4f330977e05f2c7649d0ffdef3e4edbceb02b7d841a93fda1ee9176fc210f9d378e5e52688e0c7515ede8090816e3bed13f0d7f0b495f5d4df20f9" },
                { "en-CA", "a42f01ae297cd0c9a31a20377e2c5f7ef4ca79ae1063644b92a2d374571fab5309dc71f5890b61b941500d38aab4258ee852f13c4d112b5239590646476dd010" },
                { "en-GB", "038a889b25793a1d0d68cf4826c273de08b4a73d5e389ca8c7ddd98a283bd99f4cc54b790a8d738037aa0413c204adbd20712d07d814b20b72c9b9287efafe6a" },
                { "en-US", "db06dd0f145413c84679c4b16e7a5340d214f4a8feb3c0dc16ea77cdd7985d28afb6dde8e3b871bec91dd895696b9fbd0659d89ee9069ad0a7fcdb25ff5bb3e6" },
                { "eo", "743a0507f9ffd30e86580ab236b600bc66f431e05cca091492c3315d8383799b29b2c54b184ac414673842605a0ff6626f5c4e3c370029545eb210a187925d94" },
                { "es-AR", "89c3b1025c01c8c2acbd9784b8172251c4c9f8c6bebe58e5626fa1304a292468f4c7ae4ab1395aefa09e1e8947086fa70c2e293fa71a47e31d1e6ddca789531a" },
                { "es-CL", "8e5002e021bbf2953996c122f73cb10a82fb58034f5aa03c972bdebb6ee77467caae276d30a4feb18270da6934fa7b690b059c43359eb1fa55d34a0618ad9a92" },
                { "es-ES", "673f9d2724d7c8df62c0050e717d17c40980b402b0ec7d092597738596c9558820418144bd3576532d22ab3e2b1d9d86cb4fcd956b3aa3d5f47ee695f7f13254" },
                { "es-MX", "97754c5326ca509dcbea2a8baef6d0915595a325a9ea12b390218e2fb9f13bfc41fec06c2d5e1e7fd69ff4001c686283e06f0338f64511d9ae05286eb749f55b" },
                { "et", "7fa7c4a1eea70454ba989051ff4a6817cab26db1b0bab6af06bc68bd94e7ff050d0f30e938a7585994091a5653bd66264eb4928f48acb390f86c55988094525a" },
                { "eu", "6c9e4b997f01cf095307818b9255f5a614f0519da8a210edb3d137971e03e660aeaa113c07dbed9c78e9c9762812c0dc092cb3db4322cdb501dea79c6acf192a" },
                { "fa", "7256b7f75d2cb9d39f44aa20cb65e7eef7436ee745895c0bb2e8f310d2e17aeb44c1994bce2e09aca30582d9167ddd0e5fe9820688e1f58413fdea63b182c3e2" },
                { "ff", "bd156b7a71b3e2f28e1727291682f6fe8b62ec4bbfe08f6a8f1bfe0f8ed878afeb6ca26bdfc9955a4f95e0f4708309b349928a09ace770cd566dfa9f454640f7" },
                { "fi", "2ce4a6e695605b7b9b9f3ed4961c9d472f4acd7c1a99101ec5afb36d9cd883ad86b6b48f85d997d13922d0adf9578c03e57a90b2b4c72f735376a76a6b9ff5c2" },
                { "fr", "fcb926b4771800660c0975b1cb3d0a00ac45da7bcf8ee71145a8cbfdd1c1fe26e9ec871b86446cbdffeba8a4365040d1080768f571e653c6652b359a38b16ec6" },
                { "fur", "ef195b2ab1002f5b617f0a90d424a236a0a903b19deb8fe5e38fe4b8eac10b76a986072f9efeb833be230439f4be73975dca6002ffbc2773643e5a6ebc0f60c6" },
                { "fy-NL", "b2d28b2ab86e35d49b15abf35b5ca25e3d78a655632ad202772c678603c2a74d8cf6072b78920a691ebb17940a8ffd53d3faee97615feddae94151e091ca8977" },
                { "ga-IE", "503d3ed77f551bcbd78faa50d75d186cbb34f253c580c1992e5e67033c111b265bf39718ed0cf81b1464c52241561ebc343c0bef0ec1454706f77f9570fd6bd6" },
                { "gd", "f07c5d3c918b4a02f39a3f0397a3544f78563af502e20bf3a31b0a8380eb6db007d0a4b90a26fdbf6f55ac03460ed00801f158c1d60239ce4bea7e749b8fe386" },
                { "gl", "e9aa9d9b8b1587d488a972e5c4a6bc248e379860c5b3d956ce8b654c1dfd084c69b452875ff61045c5716de05da1096ad7935f6292b7da62a701bbc2a184a46c" },
                { "gn", "4a08e6105e58840fb51a5ed3a410ce58b7c4ffb6b779d976c83a0dff9f6a90752d670c8132354b600bbe2a691d162435454151c478adc4e3d2fb8e4091416605" },
                { "gu-IN", "098e49d30f682834137adced1f7695a55dbf9ed35ba41605f2e55d8c608a512f32b43fa3eb1ac79022768396d511b5c636f31eb29c99be3eeac8e42229883405" },
                { "he", "4a825d89d47c7305aeb34ce47638956ec0b9814c727d5ef8b5cc262cd57023c19311e4a76f159ab1bcd5e9375e96a7170932f7c34e1753094395a418ef5e967a" },
                { "hi-IN", "341fc5af8e1781f2053e2527bff70b5a5ed77c39ca404d9259e3eade2f6604812fa1fc50ab6cb169caa1d57ffd0800a1b5dfc52feb9a2a70d9cd2c1ca96fbab3" },
                { "hr", "5d0c65be870ad15879c6b4d8e602a0d6d55077cf5cf8bffea7f388952bb1451b3ca7733ac3f1fc5446be0de11329d54755874a54a25750a09c3c18c91fe3c645" },
                { "hsb", "68fc766d92b1236844df56fdf68ff90caad7295330713329555b3386137476d99e5e8543c89bb16afd073c792634cc77678f4a21a0cdf6991be219d5c6116196" },
                { "hu", "ed58395133be9f37d8715ac714858259bd4b948a6510cb0e860fcdc3778409d6d252b73b423b4c5525bfe86b41ccabf734d2fad47ddfb7b1dbd47c6f8e550a15" },
                { "hy-AM", "41e1cd47804418db54de2abcfca0626cfff04f14e7fb8becfbb5bab7b2ab2cc2707cab7d3aee45a787e4ebe252a7ca62c3135511b428baf900192b4037fbf949" },
                { "ia", "3e4d79425f04b95b92ed07061ba77ef342a529918fd6812406bb5a092617fffa9b9a50cc7e56bd62d6eeb125043b539201654d2cbc906456b575871653436200" },
                { "id", "7e3d77dd8bffb06e4f049901b8898fb74f58b428b2a2b50855b01c9c058a06247a450237fdc94ae9e8d4fafde649708a74e344b8c38e53fb18e0f43829d242e2" },
                { "is", "134901d64e1c8841bebb4cbad14bda5c1eddef4d6c036cb842446e560ea2e082005fe46311efe9c48fdf7e7602ca8251cd0a0e6402ec981916ec4bb63dceab9c" },
                { "it", "b62940f70f028a4dda61a469b2ae0db1499268e2645b93304eb24b281c4a0e909b2d17f863e001fb64539adc902385948f72b553e435171d9f791a594573dc19" },
                { "ja", "a80b9cf6937959cf001b4a8fd9e030b6a0982d2fbc523923818fc8b2ba20fd03f2b6d2e5ba2f3dce14ff83047b774c37b420b7bf96447345bdc5411072009b42" },
                { "ka", "ae35bec7411462ec9477220c9065c71d27dfb2fef9e750b6bd07aeb9bbfa7ce2e012e6488e1b8517dcbc837cdb72a4480344dc04c8bf7302af1ef43b64617505" },
                { "kab", "96331c10c4e6deaf305b89bc27b0ca79210bcc425d4c9f4e8e557a01858ae6ed5c3d78a17c599b8323de905980ca7744428ccfe83889990d851915d2c9527c23" },
                { "kk", "4293ad1675a1837ba998f74f4add79cd2eeb6cb2219118153a668978a446af2a833c30ea7d1b12c90d4e0fb42d665a220bb493422d70ff42334cabf535f2b57f" },
                { "km", "8142b00ebf74ad0eced705af6d68eb348ea4e01e4b86fe6a2c7e7a35dc1d46e0f3f365af2e7bf06f8f39439313d306e1ec8bf453288b1fbc73efab85d9629820" },
                { "kn", "f585e86ed644d0c8430920508b88d45beb3f9d9cddf67fbabf83f4c280d56742716683026de1c8c49f07be7dc440ae1bf648afbacabf9f45de88eb60b534121d" },
                { "ko", "9b65ee0dda6666698495edd1665f74c4fa4462dfba7d539b5d2963beb29a339c05f4a9b512547b9969f4e6178b2a8ee1819a44f0c80d8cf37763bc385d478465" },
                { "lij", "4e2fe2b49380e20472a025f2e698ed0f553db15ce357be8004da403510b7d9ad026b099351542605af58c24dbb0fe8db4260c4529a4cc8949dff2b2967a6434c" },
                { "lt", "620ca48486b1ef411163be49c38bd4e5d82ff912ad31cc4e198c6c14acf4f9fe3ab795b8871052ead4e7f5bc9341bea177f7fec906bd0db92dddd64a194e4117" },
                { "lv", "88b1df735ba856e90ced5946d2a56fb4906d9dae8485099c952f7fb6cc257e7a20386777c2f7d1aa10e6ee30c16e064c08b3c9c3ffa0930cedfcf372ea8e48cd" },
                { "mk", "521c33864339b86311f2835f354241f5ce31e4ff47ad473aa12b9c97e3f6ccb51bf1ec69be12b17c7e7ad6cd0fd7b919264c95838d7022cafe4b24cbeee50e75" },
                { "mr", "d0cbf634cf8ed246672421920b24268bf00df32e7eeccb2781b15cf0109f66117e9d38f26e451908c793bb3236707d9e1b478af325c5f2d053f09f9638646d49" },
                { "ms", "5ef8898078ea3c07946ab3ad9e331afd13a084a99f36fbf9b3a738db1d0e435baf12054cd9e1df9ec6bba9ca220a6b84ee765e89141091de0d41a2c926d8c4fb" },
                { "my", "30c2909752ba316842bc706cde3f68f7e94ca8c77d21045d6e95e350b216adbfa9dcacb3ed2208100e02289d7d28eed49584d99c1554e57dd791dfb111aefc33" },
                { "nb-NO", "8da06555f1320a419e00ad6933a33b39a720fbc9d0c2bae1bbee6532e85661b19edcc4e8adf20258fd9d82c1a6f290d857e349dd46ea6401989a69c66c8a2245" },
                { "ne-NP", "d307d9758d6050b41d3a9c30c7854fe9653082ecfa08f6e066a3da896f7d936e709aaf3fe767d23597ff7b76272bed7558449b8cb41956d34f9628b8709b3dab" },
                { "nl", "41d3d3da596d62de5865e33fc2a81d520ec1b5a0397f53d9c0b372469b198366b8196ca255f1e4f3004d578dec8e0073a248a74dfd985563aa52308f5f0483e7" },
                { "nn-NO", "5df8d7083ba105dadf1c2ee900f8db37e0ecebb03b13f27f67b6ee7c4fc2c46c93ec3e572d87e894505b6d32360b7ae38fa4a528d010d6d8f0fbb44eb814b2f6" },
                { "oc", "ec92880d40f423ae8d9b38bf3190e322a36619fb8d9fce86dfe38c65fc49440a57dffac830db89d95d3fbb6a6df4cee0b0f27667863bc20fd34367663d5fa4b1" },
                { "pa-IN", "ff7b4d64d6bc646527102df62d61d914b081649ecbb3da4be9cd6b1af16c5df515ae440351d42b69c2992c2059a14ff5dff5121978fa8584aa8d2f6d68dd3e9a" },
                { "pl", "d9d87d9440a3839da60c51ef26970395370c20373126aaeabd9b72006aac48f53ff7478b05c98ad32c69935fffaffe75db30568635fd2a6e0550ec26ea83e9e2" },
                { "pt-BR", "cbbe5e9e07956693237b79645cbb7d8485ac963d7d5c5c5fcee90e6c8cced5021c26e75ffef2ec9ebdd35550daa5ab1cdb1c2fea64b686523b499e3800dffbac" },
                { "pt-PT", "6322be84fd6b566543cea84d63e2709568521a1e9404b70204a9b77ad2fc6d794b91876841d4a4c554835522eaefe455940f0811925f5c1e6aac0cc7532f04bb" },
                { "rm", "7cd4aa2afa2593d88f5905da6c26021e49faa14f6e343ea79302a293821c2531716317745739a1a76da8fcfcb82f5e1a6295a1d11a889a0bbdc08ce82fb60c82" },
                { "ro", "03d6abd927329b190bc9da0902466ce46de12ed2a1ca8199b1c6d8f1df3632a095087c3d916e7544e8068456a83f9ff2be6d7ffdccad66e0e2b4ed532f8f9281" },
                { "ru", "f927bc5c3618ae12f8d3deefe47e5b29b489825929ea6eb1ba4e4df3e221d2085f05a489ef8ab0b0e415905c2b0ba96e16fb6e4f688abc2c3a8ce562b0e1d56e" },
                { "sat", "9e7e801550389f65dec1e762cfde9d09e02b3d82b62fe89299abacb172bc16b0fad559e7fc9dbed214ae954510f3a9549fdc1bea235b121c8c808d1c12989b9c" },
                { "sc", "255c3a61df1acd89c8f22e2ae1c1347b3d244d2f50a0673d5a2d92ab40b000f9f04daf535e0897b4d648833bdbd8f59555e229813fefe05d3b8313e33a7572c8" },
                { "sco", "78e5812c0a4e8015d08cf243dc783cf1bc194cbe32b572c3f40f7ade5f8e197ce8fef511c5e159d8400f5c99fe5d5020f454ef271abff2f126e0f97fadd67a40" },
                { "si", "4364c956c5185659cc288bd360863741481d9943cd7ff86164cf8b6e341852e352d5e989b8ceca59deb00d3254ab1c06065bbf2dc214125384b028ae3d06ad0e" },
                { "sk", "be820434ab6a81c41df7d17d3d8fa7777f67db2cc439725dfa13f332f901f042d66e683caa0113740a590dfa4a133613fa173f67923b8eb83689c74e1fd9dcb3" },
                { "skr", "5be7676275bbbbf0af00db927cc2b4573132b5a93273983d1a36f03a891a96cf85e3b5c657a1df1c2ae6bfe7a8419a1e1d9f87eaa7f877073e59b3f07cb97466" },
                { "sl", "69767699ef3601d99411af877db4fa939af9e526fb9a4504c8a95715f2824ea06626a724fe06648a96fa82a47d8ae045753d1167ab878b1a01b216419cbf6181" },
                { "son", "2c0aca33e8d752b4a1d5c32c08a6b06e4d7b58c13195cf360227c0db85f1e0a0ad9add48f8e3419e0e76d810990e9d53a30ca111002cd148ae169565eab8f84b" },
                { "sq", "f5f4e475afdd39a10a96f5214829c7d36be716378acb94c77bcd8ff1bb5a82c17ee09c5c3c2fb292dea3b5e969bbad1263fc5bffcbd308299f2860b3d9de8614" },
                { "sr", "5d55fe0275652cbf2b899baf9d62d0656502758efa8d5cb8a729f189808967843653df39a18d5ac3b2141320de0a730129a0ca26ba74ce17b2aaf1792e4e3487" },
                { "sv-SE", "ce0d4631cc3631f4e3380f4e16bc8d12943be1e83e344288759570b35eb521a6f5b39f8bf8f429dc69ee490766e04e0fc56e845af92b99d00eb6797d894fffef" },
                { "szl", "731e2d9796fb213fb11908bc48d686bbb4f0c4ba4b954403c8772f05beb1b3db0f5c1afd0beab818a2b71ae87e03543d589716a8093a3ab714e0a52f69aa5b62" },
                { "ta", "d341325f456da63e928133ec88dd65712c2290d802d5e1782c733b6ce2bde05bc6abecefaf921c85ef4dc391f70bca17c163b7dbd7f2706697a2739ab57526a1" },
                { "te", "14f4d498aefb69505864eb0a92777b199bd4f2c588a433cefb7cce7b74e02c7374e333992a88a97bfddbea8c557bca42978b01b632b0e18f92b018b41c86ce32" },
                { "tg", "4ba20f03cb707acbb42f5be07cb72960c3a8d143a89e60bfbade692217f9016a49c685d68d303c33af6ec66ff8f52d747382d6e83f64b3c0872f7e203c1586f9" },
                { "th", "653ce669407053b7f800ffc30436d54270e4216ea2fa653bd8497fc2d2c054dd85739b6284f3ade92d755b3e8c2af53a0d1e7712a548546f599f9e855f85b875" },
                { "tl", "3b99e4c99a978638337f96ea436baf98ad439d842a8c5237f0d17f27c27c08b9892fb70f42596e875983a91465148a0011c2144cf46af8a846f9701de5922f27" },
                { "tr", "118694a0e80905e2794c2877a83597cb740db4a9d65ed2eb4e11b0ddac4770777806e7f1ebcc476e05747f8f808b27d9744c02057d64657f7a05079a2a3fc20e" },
                { "trs", "d885936808c7f4e7f884a7d3965d593e676290137db149a3b40a583948ecc2a3f8c3f31ba6f280c98fcec4ea3955f530bdff5cc0b7e1ebb7166e30248ca6204f" },
                { "uk", "d51357266dbc0af1c61a58e0be4397f8bd745fd57a54775fbc82468a81f48eca0c82bd4c7132693f5fe7c9abde0d7dac6104aa6cbe199f37c17ceb59adbad016" },
                { "ur", "edad54b45e32f3f39c75d9642764a4c965554516050f774a2861513a7c10a7f2276f10b541de2b06edcb2862f4b14aa29cd021197d78e4e1d639692aa32fa846" },
                { "uz", "7b6dc95d9624a31f73cf8258fc65e160e525bef6fb58ac6b7d130047e94bc88e25c9eb48bce4e5eafa7dc024f462f2a1e1acabda8a2d463e39d9efdb7ea26560" },
                { "vi", "d9f49921a247d02ad5b5300b80e4c8364bd8b7d73386d3de9b8f09e106e9a28de35e072a82ca4285c9a85ce9da04b248bb94268c2232fb106fa88d093c1d4310" },
                { "xh", "fe340535b6358ca6f88ae0b1ce3ef7a7d2938bc31bfaf3de07294f8f0f44f8c1537f631c8b01566a4a6ac82b6f103bc9a8a344e7294f42c5c220b7eb7e452008" },
                { "zh-CN", "164494bac12f248f2d3981a088690d9e862953eecd99c6e063fa6301102962b428aea5fa91329193c4b384889de91397aef74bbe639a6f54fe3b18479091348e" },
                { "zh-TW", "2bae808ad22ca29a693a09f911d9fc860adf0a0219a299a48fc2db105bb4f611ae81eb30a5292f97cde9c37776e1d88a3736ef84feded23a5158b0f43d2ab05b" }
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
            const string knownVersion = "135.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
