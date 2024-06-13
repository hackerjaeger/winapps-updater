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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "128.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "148e6ba840f187cc470bb99a07bba5ce567a80d2868fd72c81bdf23564e8fa15796a81dc71f017021a3f080fd4a9453c0341acef82b8e458aa34e0421579d51f" },
                { "af", "e2a8eacd00c6e3f95dd4991b28b5d113693ab5c1acce81c942189e5f7dad7c92a3fd76d330b889335662b86010e9201a60d8f6c88b444c589d43351dc52b1a95" },
                { "an", "e322c896cfecc0afae1eddeb562ddb2980e3e37e96eb7b3e54346cf96f599fd70f0c6707cf587cab2071513daec2b4d32a13107402f5b847b3b69652217b6799" },
                { "ar", "90654d7204636a314d8c21e4dc504e6ad891c05f3e582cb0c20b79ea35c6d01051ff84c85b8933082fdddc16fcaf74b717aba375599b902fd7f6d58c4b57cae2" },
                { "ast", "4d7719eba44cab572e0230153d06ac9ee7e899c2e860b3aaefebd3967bde108998a29f7f47614f2e73310bd789fc1e725a5044c88ce12f600dc44a4cae17f9da" },
                { "az", "afdd63942bdb1fc397edcdc39b21881968b4892a4c8655f6de970b33039d831838ec489878029522b9c15da7bd193f5ef869e3557e76192465e09d8da24dd380" },
                { "be", "6e75ed7c7695d100784f5456a1eceb09d4dbbb73bf66196e98f97871f6d0501eb0ddb575e8ef905fafb4cf28a87692418b7bcc75763ecee41e02bbec9aa72a03" },
                { "bg", "049157fcfccf4f9c1205f2a53e9ca9a7110425fec107945ea72d082592d15d57397b2e0806d9bf4c0d96f4f2e4c2d1b9477cc17059f6e4180d7d396152541302" },
                { "bn", "f617eb8c74021eb93eb8261f3e4a4cccb636d9e8abb7c5ee4a3cc0c4c7d83b715a24256dfc3f50278ae998454d31cd03561b99021ed22150b030f99e2d1e1913" },
                { "br", "bc2a979613a89011800c075ff654a18f47505f0100842e79899081070f3d5692f0c3c26a6d2820ee3e729b5bccb4f5520e516bf5e0f8e28bd1e6f2e0773b777d" },
                { "bs", "df1a041baa7f79fd224b019cc2e6ac2087e526fc55f61f1a7b1242c8d1f4f502df9a2151bc588d019637af2b612f679bbfb535b13604bf25d7166dfa1beefdf4" },
                { "ca", "022359c94c31409de69348d442996c8ba554776f89864c1b5c6456fede683d322ee6287039ba39e650ae0456d7de7d6e12743a4c0ec114638608b2dd36440f32" },
                { "cak", "9adce9b1e352bfada12ac0110924399c40d3d277003ea49611b22a96d27428c41156ccf5c56605fc97c47561d37aab4052c6320a41ed1f1b4c2b03a7b707871b" },
                { "cs", "3f78a092cc2d0b427463d8d8ced3bf4ffd939568f3cb69fbe4382353a4793440eb27e7a2e01a5decc62ae1b90551d1aa07ab7a8397206b2d1e249c41b31419bd" },
                { "cy", "d80295f4f8053b7b45ff4805f07dcc5840382b6beef12543c5338eedbc27648ebf09c172d9a9fa9463b47438bc4961f5aafbb98ffd20a1d1a9c692d45b01ecbb" },
                { "da", "2a30059540e60a0d8274ffee1d89328640724dc9cffd3ee2d992a2a370cd4c7fc793d3291f8dbe97b9733b16a2f6f692212a5b6cbed1054a64412906bc86ec49" },
                { "de", "eff78032577f682a6c83aadd4b7dd82a6d31c03519c63f8a06b4db7164911b0979e3cb24f172f3d9bc775a640b51c731e00007b352673e7b8150fc1ca848d674" },
                { "dsb", "cfb95d5f8092bf1dd9c66fd342db15f7c618d7e302ca3fb30d9502f41a644514c0486a40eae5a0ae459f263ebd523698ea7b46bab4b7f7713cab777a6d52046a" },
                { "el", "e3c441b230769cfd9e9b3b80f2186b966e8e5cae5348c8dc011f5bd6e378446c9e21a0d225dc1d16feb5638d3d4dfd7a6d13c4fc550732be3fb275f3e07cc891" },
                { "en-CA", "1d961858ab992269257f4b110deb45fe6ae5422fa7dac41519178cffd4bd26be44f56e6921236cf44c22e9015935bbc3bc080639de3850a6f0253f3b46378f79" },
                { "en-GB", "90d699c033c5486deda40e62adeb88a8a6c0d9bc362b17fa1ca2acd515759eaf224f155f3f780e641650978152cfdad6d07c5a266bc1e244b17b4bb5bba3487e" },
                { "en-US", "6aa9c1aaeb77d08395271e78de97f5dd809f21788d8d1fafd35113511dd77bbbe2c45e6e02ad4f73fe308db664b034d17b741df415493207cde62945566b71ad" },
                { "eo", "851e4ae843500b9244a711131580609dd5cdcfc2001fc7c82a4bcee6b66b9a8dfb846aede92d3b125bfd2d9309b7a1e4877a2a90c851f2c65d71a1aabe902ccc" },
                { "es-AR", "a697d0dc9731b3edf38a782fe910fdaf90b0c62c7cd35803d2bb9a02eb1e87f748e6d37b3d3274f51712039f38bfc4487b1e4697945501fa7e53478f69428e0e" },
                { "es-CL", "e4813f50b03559c747eca592fe7f36b8b1e632c695b118f1895a65d4a246b5d1c14b2d8d12a98b508123fd6e33ded7935999e3b81b34b574d2b7979d6bf4924d" },
                { "es-ES", "63afefb70faf8f467059cf14c6974f4f4daa580c9c7dd3b5cdca27553b577b06ddfcbdc7659df12f511ee87b1a15b78e6ad683379c68990d40eeb30c1f46ee44" },
                { "es-MX", "bafaecf6f9e9f5df48973430f16f9fff46da978ed9eaee2495a4673d62833261f463ccbdd37ed299d02300b093491cdc5da8bc316bcf80cbdc7fe945e9ef38d7" },
                { "et", "62e281da8d2a350cb8fed28cea26aa8965e165d1e53d134c00026d2897ccc6f8ea7140aed642c1e9b33a6f497fb0b506aa58702436616d470ff86fbaf225b38a" },
                { "eu", "af68a7776eaf7c533845a99d6c1639058d636d4f349206d7e43e0106d2d70450d955628c14385477dd603ecfdf5b17942a617a67ea0e5346e2de327d69a20559" },
                { "fa", "55add62f4545ccd0f434cd07b9b8570e7e0305711c04efb18c15ce210bca52984ce951bfbd5dda3854fe0347a94c90dd9c98aa1809647bd6f762e5655c0d7da9" },
                { "ff", "dce546bf6df8ac625d11d6bb5608f4d49015e4fdcf72e3280d90d7ba26a989fb1f1dcfff72800c53eaffd2c88b334eae073918733fab97aabde3b1a293605ff3" },
                { "fi", "39ecc8f045cbf91ceed2c1e8411462dfea32ed8058d1df4ec6cd2103a8505dac22fc916ee95b478eae03ff2fceae31569b1db9007fd3096f41697b86a82dad06" },
                { "fr", "e52f998ff73bdefbffa144c5caaf9c7fa9c5d2138165274fd97c97014cea2d426249f7529727b529e39d1f98c6461077d40828dcf74e2b9c64703849805b02bf" },
                { "fur", "ba275201be9c4ee9878cd62bf9f0da9a7df41689f10ea5e5175c154ae73f7da787bef37b77190430ce2ed056089aafd48844fa53c258388e3812ce662a298627" },
                { "fy-NL", "8e87a0fcbfec871e45b8d91e9948237ca183297ef2dfcc477c073d394bc5ba11310d7e97ec6fa560900f5d8f09e4ce1eef0c6ad4e369826bc9d1c4ecea1b86a5" },
                { "ga-IE", "10fa11cc42997b1ce331de9917249c99ca2548e18b1aef36de37c5860a4e4b88d1afd3cd871b531519cc35bee7d5bd82cc89a84ac60721b51a23c01af617dfb4" },
                { "gd", "6ac496a37165b3cbad65b6840c64804b4560cd8e491214cfee3c949d70a143ff4bb0641b53ff8cf6228e91a5e4313af92b4a3d5d828aa96b3066d9f42c3dd3fb" },
                { "gl", "f1dd8394bbbe55da6a5defc499860d37b487be3bd582a0307d59fcb5897af4fdb95441f4d00a2f4e88a6e558124afa244889eaeb43ea482db5add424e32da0bc" },
                { "gn", "ae58d601a6a39456a264ad5b185bdeabd5370621fddf58f3893a28acb68e373d263de7f9d29e8b95552fd7c0b247dee452d2657cba9915e47571daaefe29d758" },
                { "gu-IN", "54792f0abc802057d2059075110680b153384ffd69771a3d60a84c1a05fb671bf608dee17fc77cd22f19b1a0e6c8410c6dfb4260a240145ba2c474fb82f4630b" },
                { "he", "813bbd798791b47e75ec3a4cf23b11ce6de95e97059ce4f5b46057b0e445be8e461a0a4f986fdb07103bed6887c7a6c761147983b2860b20d1b1faded6bb3337" },
                { "hi-IN", "b26c82e2f2e2f23b8730e813765aa42c30b3a8238670a93c99895c09115b09cfbadc8776c5b3d858eef970a4c902899cd47c4a47cb85abffacc21ecaf9387f68" },
                { "hr", "3022afe6781d47c242ca06f5f3db195a9ae95ed43fca818454c2228cbd6e3da2562b15581766ddd62248954d8c0655b369c9fefeea2674173bfa2ea2a6829c60" },
                { "hsb", "55d3f8007f8b34c1441cf8453a79f776ea37322dcd3ed4bf68c3ced9faa81a96fb3a2c0391ab018ae3657b57feec78213993372225bb60d6c2ac1335a3684d9d" },
                { "hu", "d59905cc1d1cb6d61bdaf8e7ebf3a565a6183c17f02820cda02844d0f58252b170ee64898a0905b27bbf02d180a454e42c2cac156277fd60714a4d444be230f7" },
                { "hy-AM", "2a23bb207ef1f616971c6db69346c823d81a7e7c63824830e041b1f68d440ec8d1a3966c1b429485c6e9927d1eed863f049472a35085db7b5397ce58dd675ce5" },
                { "ia", "862d75292a2fa4ab69f43f71f07c2e94fef46a4905ea3c4a0849b3790c76b2381a9fb222d97ec01f50f9767ca4db96a853b9bceb955c834a5c60a092cd5fdb6c" },
                { "id", "b7f2fda636c05980545696f48180af20eed274096cb7c4735991b40af692e4fa4cd3f2d40c97bcfea64779779b7d2b896445ed02b357ce291685bab0550d657b" },
                { "is", "25600d7bd51d7b04f6a099a111204d33b9f58fd813a98b624bb1a8c9923d762c625eb459cfd94dbba1f61ee4b559cd8c832256ed5d102aee621da225126bb259" },
                { "it", "84cbb6a2097140a36bd6d419ff135a28aa0c7e12036dde4e9a423c3f65686d002dd1f40bb5d91bd2b4f3e341d4c605e019756c98b4d3b0ffc504583628c9512a" },
                { "ja", "8cfff6f12482d1cc0531829c758c3b69d9c2613a2fb652f0d74ba2e4b327bf06913bdb96da4f77539b3ce0a4a30751cc0fb622d5426b96e77adc09fe0621f697" },
                { "ka", "0f172d326bbb033beb80d1948ecb9232a30de6ded6310d5a491fa0dbe34681bcb18f6c99e51cf8281d65e7cffaaf2484ee18660fe22cdbe267b8cfc8d00d9717" },
                { "kab", "915eae7cec918ee46eb8c4a7b98bc179e940b81359028bb1081424705555d38aa37c332d3fa6e9dcf47133a44c36bf65d7da51f7f0a88c5dec478de803e8eaeb" },
                { "kk", "3f2217158378555731b52757dcd3ab1585989398aaa460bddc76ef36ed8f03d88eb0160ce68a4b7866b1b5a7654d65a335b12dd0597c552d29831de65e9552e7" },
                { "km", "6a7a12db149e062b5ed88f4f8bb788ddd019ce6eb657c4ac59c45e8d5537c048bca07953a1785fc09c5debe5720d94a52066b2d80388a11a33b58bb5a2a8932e" },
                { "kn", "930ab790900f4e97f3bfcdc1cd239196c36c90de1d65029cd153a15c9df23bd590914c3fdb5359c3e33866a67e7c287c6cb18641e08e1ffaeaf9055ae9956988" },
                { "ko", "c4cf59397295b74cfafcce7c2972592a56959fcb4b5d24e7ead9f012d4e1a0efde92410b629b965d6a5104f0b3ea9e9bb72b45f5830fc447d88fde35ecfb793e" },
                { "lij", "d4ba9315f03158c57be389e73425030c53af6c193d551149a7daaa92b046b80c70bcceb98b7ef22775d634a3bf477832741e14c0dc4d363e2bb922c2a72e75a1" },
                { "lt", "de90fcf25db878fc14fba268a3ca648c27d1a86a09604aeba62dea54de8baf076ea692244ee21dd9b114bf9046eb40cd9f0eb4b30ca3fa6c67b018b47ecc3efa" },
                { "lv", "a6f7c77661a8fa73ec2b5b7408b089a1fea56263bedb59e6daf9e5904cac08cb4b99925a4443bf876232ef54ce24d70f31ec1e0b5100c34ab38b4013df38f875" },
                { "mk", "101c9b0dcf98bc3e4023dd06458ce076b836dea1971cb4a01708ac3c7ef48af6c2f32013ddcb28d0a9ecfeb6e67ae336d65c5037dd6d58387eec1a33863abbb6" },
                { "mr", "ea812f73688c1d07f95fb6ccc8d684f1427dc7ef6481f963f07c5f19b858aabca5ab3f00c05fecbe4836ea32008d408b3e22ffd0e65527e8998a25f048033700" },
                { "ms", "59222a5cc7f0604bdbecd570a21245f67ff48c77180e4b29a347bf75e1d91620b4cb2c9a4719c6ab4bbf4ebb2b5852379418db9a319b42f6cb04f093a3d413de" },
                { "my", "5b748ea0f742d4923f764f4e06f8a9e6eeef158489f4c5126394a123af9898a0e8626656df3f4e10b967ac25ece5d6d51d51f186b86047c64e600993a70629c0" },
                { "nb-NO", "d79bb62d2869a9d48d49ef7f0764c8005b150fbcabcc87de583eca7dec298fde00a0cd10d5762fe80c4be94d078f79f6354beb4a1e64e400f4cfe06e4a6903a6" },
                { "ne-NP", "a02fc55f76ab478e747d2c94d3b1e49b321e63c45677d161daaea2a028a424ca9e801b7508a08c268d27041e0dda518f61c942274b83b8e5dd417a2070f568ec" },
                { "nl", "2f60898210971d32613035e13a8c2e9b092550fff2f136cdcd222aa7177b121ed2adcf8ae80550773ed36e9bc5a1fdf1dd5fad4ac6d4ce24876d6fe928e7172e" },
                { "nn-NO", "6463ebf99ff928f7bf6b1bdbaf343eba3c207e5fff23c1ea54f45a007ea9c6efe56f7856949da3479e214f39def8c7b9cfbe3c01249c36e69f57abb2991551e9" },
                { "oc", "0a4a600b99b620726bb4394e0abf980ed9b59158705697b32c717c05aa96270a2e40887a3c99891d0f055643bdc61afe41fa19a0d31ff65b98558d0d20ccb86e" },
                { "pa-IN", "149d3a2082f5142ca04326d5c13c3020f0c94d51e2da1b4cbecb9e6769cb32b59d04916c132893bf3c440e050a71855ef0a4412a7e3aede9d73d546733c90485" },
                { "pl", "12327e48e6d2dcb20ab5162017950ab155270977691cb22fdb537fe77e02578f6756fa44f09d5200b08d9b41fb4b08b2bea8d9df0594504c33b8534b9433d0fe" },
                { "pt-BR", "200ba3deeb1f8dba36e9982e1745f4bbcbbbd0384de1f86fbebd14878c070893bd561fe66add28395934d13f2baf2de9681f95d3a4b39d1d039201ac6b1c8e35" },
                { "pt-PT", "48dd3e2c89eb386183065652575f543189799607ba6840425df439af3195425831b050ecb6e09e7e5cba0cc4625d74a2dbbb36bda19dc9e865b63d3021a13b75" },
                { "rm", "3224c158f835cf9b44c6c53b2fa479b1813ae6e9cd0f99a71d9aeabb80bf3106d10d6446fb326438f0dddb97e29ed3ed597619fb0c10f815768237edb25a934d" },
                { "ro", "487ccc98f5cf61d3e3ee38c314cf82ecc7f8fba9ed8450a63f32ed2ff029625c057e754ea2e2a94a1ee88bcd29cb4715fc726889ec13bd123f76db7316fd8f28" },
                { "ru", "e547266a3b7d133af74e83297a7b8cb0491f7b39e2fd02f71ea61d159ffb7e22de95771769fed610e5ea7ec383264fdf936806d6b587f161bdaf404d83ee9b58" },
                { "sat", "c1b6ff17a60390b2665b81011f973c51cfde39adb2b268037b80426cedfb02d95422030d5168b36eccb4f988fa8a0937c79fa484ad4bc40e58c779172d4bcaf7" },
                { "sc", "60051e60f5ea8ac1e64cd943c079dce5192a1a8a309cbd8dacee30f4cbbdc18f65afbffa6900b82e740dd88f6904a22611acd7906927e7c4ae9a4e5460cb8579" },
                { "sco", "76f03532c3cfc12e84c7ff890a1c1a4e85a0dfdad52c41e671cc8b016ef2acf25e0698fb24acb6a60410aeba0a739b611d976b272be992d30eb7ae4a4fc07717" },
                { "si", "3ab3850647ce02e1d6788ba3f919dc0c07ff59045ddef1212bbd0e8962df24c613a75c993f12e7d9f8af3ff018017390924ba245c425a038ade1185c34b29768" },
                { "sk", "d1ca6f90fecf167ac00884195df346e48fdeddc85d84523b28b6c8d39b23eff93e84c23ce40dd5af64c0a4e0df658060b87220580e6a1a53a7a96bf0281029d6" },
                { "skr", "c4b04da8e3b4f6d04b061e44ab350dae907e6e974bf17b16ae457bae14492a14b3eac91b3b4a26279200988617c0dc9bb29e047f1828d424d86e77535d32c2a8" },
                { "sl", "df59a75e40e5fa8f4cd2a0312bc1223d49711f17085183d56aef661f939567484cc928ab9aadd324421c2b6d36ad77df5396ca2c0877939e4e78fd7fa78f4d74" },
                { "son", "b45a758f86d18c88bc8377888fbc0db3e47a93de5d0352b81d293b8c5ef957fef15c86a096df9e3137bdc6e641e80dbed3c38e7932ed44dda1c11cd657dddd1f" },
                { "sq", "7bafa9899ee7d71f0a6c18843a65978d66091a661f58983c3502d2e24afd1fa2be338efda14cc8c1d44f16e7e524aec2255e66bc2e55105933277c85e94af985" },
                { "sr", "3d73458b4e136a5d53e1ad5d76a69d1ee41a3b265de5016821bd8297b6c99f3da202ff046af62512b432d017ad7b4741c5e5b3051395422471e562e3bb16090e" },
                { "sv-SE", "7e42d0ad14470f51fe3ae01c82e8a9e8436105d77e9309606c901af83eb4971072861da3a43be009939a2c72f27cb48065253c9c9e46267f48153dedad930ca6" },
                { "szl", "ea47d2d2617e7cb0177dd50be75841b74bbec37fe2065988ca0e9dcc778e3d550c1ad0bbd77a8c1599037b0f17c6ba543ffb18e431a7261d6449d2c0316a4a5c" },
                { "ta", "b8c74d64c3f7de3c9c9eff8e889c9954178211fe8f2726862402fa8f7588bbbbfd69685a91c4095d9e2f8018ea9df4bbc75f6912224d2f63c0c6e28434d4dc85" },
                { "te", "18016a76ee9f5840ac91b03e549916d6b11d3b0cc17f8eb4286d2580c38842af8a1578069d99228d608431f120571ccd48001c48b8ad8f6892ddb5863d4fa08f" },
                { "tg", "c437bb625010ac3ba6b471437fb76c90065389a452404781bc4a20488d80c579dc2ba25397f5cf150f6fe464d4c266ae5c9cb05193389db961c33c2ee6002f24" },
                { "th", "4b13d51ff69a976321533b7689cf39ea62280148d13dfb67c805d2be2ad7f55c0a4ec4841a73a9bcc63f79902e40956b517d163d53ce239a862d09c901e519fd" },
                { "tl", "460013bc4ca785e2702b4c50233b6e516559afbc5a962e696f15189178e7042be406c065ddadfb1b11ec2438630516419c32169ffd6940fd3676d8782df6c6b8" },
                { "tr", "97a18f2a9aa8fce1d329c88640442633bb06356f1444aac169abfa4a7159e91185ad726e221b622966c31b678d7f222611ea9e6024d006c7f73d625028b74398" },
                { "trs", "be46fa557d19d7db67eec76ffb0eb1cf9a5e59dae38fcddb39486b1bc62494cdb0750375462abaf1722bd74d529f6ee1a5dfdcf823ccad29ce3cbeff056b8de7" },
                { "uk", "c852dc9c4fd78383477a8af0150e4e1b6c5bbb656da566108e62e7ba789dcaee85bf1a112ebe34462e52503ef2ab0b9001f97394d8f2521afe7b1375da6cb18d" },
                { "ur", "db5f64938a0971ec3bcbbb965a1f71b3325af5bc91f82d7605e74916979aedb188deeb8433afb4a29f26aab214b3179043c4dad974ae1bc0a38a9309ab0e2ef6" },
                { "uz", "2ac874447925dd82bde385dbbf8b47cab8066362bc32a1754e2cee2d33a49812f3fa05daed3973799b65a50a8717186c73558ae0bafe9dc85b4b769b4dbfc3f7" },
                { "vi", "8ba812944c2bc91feb732683e324fea3b48a99a70e4732916a26c6f6f7da32974d8e694eda5d2aa7fbc7f19b6118e115957775e00449f50bada988fc00adec49" },
                { "xh", "a878cbf51a99ec3e44863623324ea66b52b5928567ed003535617c4ebba585da10804c59c47fdfdd8230fb14c382b3f02268455ae785464a38460a8193e24db1" },
                { "zh-CN", "951b5168bf38228805d15f09cba7ab69847c51929cb47f1021ceb6fe486aeec7dad92e14da975c471f4b54e78e304ec3c65d79162698f6159375cc6cd47fa4ed" },
                { "zh-TW", "c54ec71e0264e99ef8dcb679ac97e4c222c12cd0f00ac0103ae3e89b98584dc392aa3f71a3c3fcce029717ed347835b87b8b9db58eec7926e662c55f560499b1" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "875fe94ad4a0e876eadc59695ea5360433fc968917b541a1ae15fc4206638a43dde89d083a34260e517a241ab9416d6601d037625f557876efc8d4cb5933d7af" },
                { "af", "4f506c93edce48e73e8b2c21ea8afe6d481145532be0ae77970b826dc8641e27e994a6cdf4ace18b84c2e3c4b403534a116df3550fe4386b76558eed0c4dc07f" },
                { "an", "262c4ac46a7fb2b63d80e72bbaf6f20371759c37ef78a24c7554ae3017bce481365feb38d00cdcd153a5a527b19a7fcf58f103dd19a03be8f782f81de952c0bb" },
                { "ar", "81ad3203c8e4d4ba8f2bedcab78836150300f9dd1220cc795a9fbd6aba9c58f53e4197234d90b1f4c485e509a133ec4548a167a4729962afb131222ba6d04b0e" },
                { "ast", "238f4b8147be04687112ad231f29e994ba7d54fccb94484ef22264baabbcf48e054236cc1954eec69453ad14f00cd068fb210643c48592f46ecde98ced978766" },
                { "az", "c2d8b655a48669a4c44cb4f7659c1671f5189ef7124c6271f97bb22e84f393c52d840c54aa6118edca76e8e9b78f44c75837c94ef909b5f64d5bac33d4b96465" },
                { "be", "c5ac14ed68dbdd18444987aa19a0d06d72a75bdad8a42200bb3a21ca48fd27050bb8b27c6f9d858bb691b4c2ff76111ed7028ebf0229f9352e0363491636933e" },
                { "bg", "66a39b9fa92d8a8f75c148cba4939fb1667576ee99e4e4e54a5aae23cf18adcc43ed21c4fd11af8283839a7a9f8d1c6d1de8e52d8facc876d95f568a10bdacf8" },
                { "bn", "c581918077886398eca62f07b0b2428b4e96dd7cd021e2fb015977f1dde621b458c802498d7a79308fbd9bfec04f29d86ed63ff19061d60a87aad5e58a33d5a5" },
                { "br", "aee9bc93c621cb36b343b8f0c2bd49a3cb4bc8c28bc7ba5418f96ba19652e10dd522a1ad4cb7b247a5470751c211f6448c704731dd27fd22226f2ee971d5713e" },
                { "bs", "52d4cd03c225a2d9747e8ff457635d8d3661bd3318fa117f71db5653516c27c68822c851fedfe13cd6441bbef2c77cc82c43bc19fcf5a9cf0b71f374c073d63e" },
                { "ca", "2570ea27b8090de5f5afcc248a8385707054b39ec51173dfd1fee077003a50a66c82db6a9e1837d927afd96d1d9e6a2546a421c9973e3f6eee5bd46ce199295f" },
                { "cak", "94410be415336ed7de9677da903c1ad24e8f6ce45311ad9d041461759c7ad59bba497b458a2e8cc17100d6c9a448f8208f84bf9cccece5d074eb504752c4fb56" },
                { "cs", "86c336e1c740e4de9d55de2a91c2663d6a70093bf07ab780f5804fe0a76c597875545fc9b09e6fb5bf93ac7068e2bd2435721965c24f4c3349edb1d10b71fb29" },
                { "cy", "4a3e8f5ea65dc8659f4514efe9020201a5391d992ce6c1f1265d98ea7f8b3c1609c0e6921219f55dd71d361ce7deb07dbad62af94f60ff63a57502707dd306c9" },
                { "da", "fd914f5e54d1377b4048952c71aeb1135c4442e723aa0d9762011d72b98054cd2feac25254247eed841ae336bfb7aa558a7bcc315aa8ac17ab5aac7927c99247" },
                { "de", "aac38b0853000afe902d031fb2d3eb3dda6689c001d7b1e0dc6c3b4344849de52c759fd2f0d73f395fbcb4089ba67a95c831ef500b55a065e3027769365da8af" },
                { "dsb", "edfee31fc0ded062c4f46e4467e3ae15869baf2597a73f10ece169c0e0a5a46e6c19ebc9b603481f663d553312f4b8e8615eebc38690d703dd7d778f10dc58ec" },
                { "el", "a6d7c971e0b1894504b110578421ceb3e0d798c8d282fef2b82898806e9c64d5c685573bbc64bb656a7c4894b28993107209fa87c1f9a321f921f9af8d932f34" },
                { "en-CA", "2fe0986b187a934f5dd545f73bb59303b42a2155b743031433cbcdd9c51463d09be036003705b18945d6819f02ecf0a43caa7226a42ac84c8f32ef47138cb80b" },
                { "en-GB", "b02a8bdd515196ca221d7d937e6693beff0b1d7797af56c9c27baec167c6606611aee4d773bc807cd9eb00bb64f4886faab94d3a54f6ae32ca6822ae1d0aa4a7" },
                { "en-US", "a66ad2f8d26f544924fdbbdb469c54a02ee5eb6cb2f6ce0165bbef05054cc292bde3daa7808181efe13d99039cb674f578924ccb15dd7a7854e008d4c3c824a0" },
                { "eo", "ed8fb0158cb9bdff9b717648ef81a549c8bfa18a76e3cf3564d11eed80a6e629094ed68f7bd9b87be548c64084a84bf9899c1247ad719812807a5ff14c0dbfc5" },
                { "es-AR", "2c0994c59bbcbcbdb1f641dd43fc895528a2a1e6f386a5befce3d3dfbbe584f460d687967fa1a29355b22a4cbf1f2a8f466fbeee787f5d566414e63580596365" },
                { "es-CL", "7a0b3b07118650ab892fa90cf99cac85813a2f19ed6d68705c171de2927b612e292dcf7c340baf6c3d3f9651e655aa709553c09c8973b78a8f92987e6bed1d71" },
                { "es-ES", "ab6a631a503b160c049929680a145eec3720acd08e4321862faf2025c92dc293a00de7544eb5554a4d56187a9d71c1dc88a7b9cc2a7b261d378b5a7be9a312e7" },
                { "es-MX", "a732e4d1c5fa5b333c24c73c681b4dcfad0a72d81a8b82cb4281b006cdd452b60cebd287494c43767791cc653ee3b6f222a9bd3e0d71c3c51d3bc3656773cf3d" },
                { "et", "a7fb44bc0f502fef34e79de06b21ff37d1dd14c3d6c3252886fe4a06e5b174d0078845da632c33643893466ca19cc84e3798b41736878ac25a2b4005266dbec8" },
                { "eu", "c96e37b03c378ff6cb1966c85a7d8a7bc209508edf54c41bae7959e0b455040e1db63ed2fab365d224105f990a5bd2b8b0335cedc2d0dd2191a754f98cc4f6fe" },
                { "fa", "c751a2499c3e36042bca299c27ac90511552eeaf5efc7603b1aacb45d02cb2a85340c360621f22faf472eef4cd97d49050f9bb5dfd3cab4e5bf57c6ddbe38fa2" },
                { "ff", "b9734f16632eacdbb6a7c70249ce260ba53197172cfc5cee808515239b9fa14a29a01a55f34b2d85cfb4341b69d7ea5a8597e85be1b3b74a0b2fefe9cdbcdb09" },
                { "fi", "78063c1e5c69d4f0c34fde94fb2fe0bccf3daff58e9b99f227ad4b018051f4fe1bb776bea326f98fa6d6ef819b4a874bbc9d3bd53e7ebfa5d380f2a493e5034b" },
                { "fr", "0b96f48e723478d73ebb147ebae9bce7d6dc37459dae80850575e636d7f73c10003d7ae72ea0c24e075e54347345ae80a4ffbe6b1824fe24840c812913a83add" },
                { "fur", "f868d97ea0c213b2f1c98460bac7650d99bc54b7669e0d605f5cf66a8902b40e1718a715f6fd26f6fd1919fd5dc93301e77d258cefb53e2756f013dc99ee61ce" },
                { "fy-NL", "7ab2ed6583e2480dbe97de7640216bb0982760c13f91f32cd0c9644153caad894d8ed6f32fbb1c12b4c54110a3e917e98c25366c7f029770b7eead067e7bfe4e" },
                { "ga-IE", "5c38cd20283891a511569fcbcdec891dc46b978e2997942a636f41c1eed3e5f1205d3a9320cc5557bca036b9a1394a5b5f10490244500f9a582b0c1cefc9060b" },
                { "gd", "bd2ecfce56f1aac6312d4f8ed103bf70624bd1dc062f7346952f1b793f5a0575e13c4ec7c2ab22650f9d9c7b1f9c3848427dfc6b9fbe869e30c112794a109044" },
                { "gl", "43f879dceb0313b0201cbad7c0a5c4ac200ea131f021e3e3856db18a0d20ed325a4d8d2f0dc8f72e85bf132ffe5d3c156d3a8a8267e8aaf785c17815b31f8c48" },
                { "gn", "61906da6ae4894c3d001e7c1fd99957e3ad4bbcbd94ac63cec3b7c25d12b4be2122ea07e098490ae2944bd9eb812e54b1ff0570ba93ab5f954398c5fa9810588" },
                { "gu-IN", "b51b0c7be41a863e4c468d478c398f263e7cfecf0a5c28a926e781512db967aef10a01699a1dd5498803b8a696fead023d15f4024cdd4cf883c010798d7c7ccf" },
                { "he", "b4bb75b1fe4b2f576bce2da04a1879d195986f579a2b12b64aafdb81bd930a2ae9a90e745484c1fc878b41c90dc150ea33c967f03f340532abeb2174ffdea241" },
                { "hi-IN", "539e548671b149d167f519ad0050a334887a8047bb43afbe02736b0855ee63d6a2c273d0ad609aba1b56fcd98ff642bc0e55e4163c72560351b3a3490d29f85b" },
                { "hr", "674db4957bb4b3cef8fb258fbd22313884391c3d62a9507e39aa06d68f97b2652e90f25fe7c52134f57596d0daa95734dfdc1f74a54f0babbcb50247d6891205" },
                { "hsb", "e898398bab47810e6cbc1ee4d230c564f6fcd00a357a2ad8be42ef6380c02e252f4f4ec5b643b9c2e86471a2aec05bab1af9580d22671e729e86754f981ccaed" },
                { "hu", "f065925a1d2c96780930150ae6789ebe53a24a93938a70920902d9785b758da2e34cf14ee98b904a96dd59619805c1a60c59213d1ce730cb763e010a32a46ca2" },
                { "hy-AM", "e11a373ce3521ba621f53991048f46dbc107795091223f812616553e0719343082361b18659a5365bb919cd4136e8e1a5aef8af7b01a4e5486353fbc1621d3ea" },
                { "ia", "33d4690e17e56c23084d2f758e8982f31aee89eab94365d4a6b617f28bab0651b5ccdb2dbfe546f09a887244c56ad7ae7ef5bb6ea22d7a3dfd0cbff311c932f0" },
                { "id", "41d40f32492adf70cb08266ecfd7cc6df0e4149d85f9e41ac51f3d1e36bbba1d6ab7834332a9ce40307f2bc019fd7cfefb3a575a181717be1bc7a35564bda6b4" },
                { "is", "1f285c260a5e5920e25ef633e8ac1da53f59a93b9b313f480776856c232137739d0e5129213416b4747c423a323d977faf6a60f8a8129351b9ed0d05e7ac21d0" },
                { "it", "d35ae5c4ae741dba7d149d31fff7d8147b4b723267bffb96ed74caea24a5f481e03a1e1cf00fa891d6911e7a331bc60bbdfed33d63a00a09e0676ffea6b7b8e6" },
                { "ja", "36363479118a60e58127aec489c4b61fd987e06163a27c899d4a79ed6bddba96acd4dd381c8c6b035649f78dd7fdbd5594a2264e933616f47d7198d7f8a3a87c" },
                { "ka", "c41b3d54061aaaea8d194dde0f08403cc021fb2ef90c519f4570e9d7fb471e9e0f297197abcb753eefcc3ab1522e23275d88d7beff55eba780b1882bfe41a2a1" },
                { "kab", "6c70b68f36dff7d1ddca366c34dd7a80175a896bb81646a4fe8f4b12f4b66b3f0cb65bd773cbb4109ab605960ddc7ad4257dbe5956f2223d22a248740f79e647" },
                { "kk", "3ee1df131aa4c52fe7d062b8573eef6f28b129d85fbda460faa90c56c8f14e27c35f16b22b4a44a8ac186f2d35b6a960cebd188df84e3da7bc0445a088b2adc7" },
                { "km", "9457ee77c5e4853026dc7299bda25234fe19459eaf43cb5425ce1c76f86d7c293c81d1f9f8b25d4326014b8996ce1240a876ef7f123c2fec5074512e6351d5c1" },
                { "kn", "b1ac740fb7dac330853e256b32710ea63ca6b438ffc16a5fac570f7b829e24ee41c90c409d10c6769770ef13b67c388a5b5cb7cb956020e1779f39851f6a3ce3" },
                { "ko", "e223aba00d1cd4f2466056964007a734a4e25fbdb529281022c27a37da349412c7bd0a3afc2418b06c345613a7ecf3b04d18178f69c9dc62fa38cdd764bf5e86" },
                { "lij", "6c807aa12c7700250e07cb8d8b430e5b7f092b719cc97ce8d6271495cba85f5edbd171b828e496332c50fe46e2be79cfbb8c1599e1262eb13c368b883d2b21d3" },
                { "lt", "a50ddbb144279cb65bfd65856b21fa081b0ab2b2f9a4d299d8caef18db2e62df1f51426eca94cd83065b8369654c6f512b447a8683a14903b50e848ea905920b" },
                { "lv", "4e1f8058485621245e0d4c75bb504475561756fab2256e13eb2239d1d3b75623d2771c3256e1f3ce7de97e7e9771085dcecd0a21e982f3348755bf49ed39bfd0" },
                { "mk", "042d8dac4760a9dc4b34435ac6b5508a5c5383f98d7aec528f944ac9bd95b46c49f12d9e39b2a664e3a8537f00bc1b3828fa1622e04b6079c9a2045a6f3bce0d" },
                { "mr", "abaebb7c021db5e5e8ec072b4932b78575f33cc2c2ab5f48efe7fbe596f9e170b720e2cbb4dc2f3d9735b8253f4e4898b18d04b7aac0f91c75b37dec3255799d" },
                { "ms", "84f07442ad4ab67bd3c978c5f0afdcb0a733d52f6957f4f54b6ffe7aeea70ff76256ce56aa392db6994f471a637e274c8a574148b5975dc0875f95185b75e462" },
                { "my", "f1620cbecf0856c43e2bbe5f9becf35b91fb28d877ba1cfd3ba8149852cf647955af91ad7c336231a5b7e2862f67adfcb18c58b20b7dcfb2e5f6b83eee2d3d4f" },
                { "nb-NO", "81073dcd0fe4c38bf1a17d00e9507843a74f620b9862b3d98a72319349ab258ca8183b066329e58401fe20fe4d6c0295900c8bb26b4586448d3dac51934a713d" },
                { "ne-NP", "098ef552663ab8b275f711183c7672fa1379da2cf78189e0159341739d686ad0c4fd22c979ddcc46c20c50eeb961e8174e1c9cd2a1abdf7bf2e3f6113e0c3186" },
                { "nl", "d45ac4e08aa2543409e3eecc7df9d66c5e7ff284584424be9d69a5e0137b24e3bdf5ed21a9b4cbd945abcdd6a55b11c4784b55d7624c65d11cc1c3e0a256bef5" },
                { "nn-NO", "4e551610e069d695784ca650af12ddbce1e13525b2b6aade357fec8eded50544f8ba018549bc4c7cb5d35ddb3ff0027158803717e377cf617c5695dc46385ec1" },
                { "oc", "0786c50bc6dc0e66e0ec9ded6d577a32f409f2cfd8f0c994cdb191beca8bd2e0ac17ee0604a28244547b5a44051a6a0a6662da86829022632996bbb5a784e79e" },
                { "pa-IN", "4da805829cd0dde8d0cac8c33a6380f175d612eb7142db6c863eeb01c0dd62a72711aa4098002ae8764a58138d3a8aa2c39d1f6910ea4dc9f03d1e968584a845" },
                { "pl", "3a930b1fed5b81aeb9f40eb865abc944181753ffbe87bac9e7b05214a71ef2219de37f58b7883a183f194108d2d991caeb324a5ef0ad4c51f6619bf97a966def" },
                { "pt-BR", "129d785d014bbcccce69ac51a266da4f3568fe60b26051ef4242468a610ee0a64d71860cd174d10d2b5147fd1bf6035f9c9fc6dc704239d30571757abff68342" },
                { "pt-PT", "ae00f39f268cd8fbc041636a03ff5b5ba7a111d9efed207dcca50e467d12c8690931b07effb3fc948def6cc6e7ea7ae6644c654db44634898eef7b79bc458358" },
                { "rm", "c12fcefe40c65caec84a6c203a28499831aae975475b2c1cbdade8f583ebaaecee139e93596d8845b5143eb9cb7e683f566b26c6dd70d59f25a5a1f7af838f0a" },
                { "ro", "43d662db49825ec72d6831aa2a804f8648d71b6ff02c15abf589c6bf9f7cb18bc5669153a08a4ae520c39ceaa31fb8d55f1cf120d2bba0f41a0ea58159738e81" },
                { "ru", "c23d373911faa8078909c740babb803ced2d65aff4647a7658326fe1c24be93a6d0d07391a2f794590787f71d55769a6c6ef386d86ea0303d7fa3585971dbe42" },
                { "sat", "facff154742543d4aec7fb77c70ee3aca73c4463af35d819ff7e153192c03cc7612297736561e77a99ee9e490e33e3243a32a7df7c2c2117a38884b13991b3cb" },
                { "sc", "86df11e22f2f9a658cbce1af34d01ac9566143112e543fb005feace3001005d4451303f03ea0e5ec4c80450f86991084dc065111588b217cdfe5edb59fdd09d5" },
                { "sco", "650483cf44583c20ea5f0fe38246ab1f50d162c0518df941b5be063c24820fc2b4a77a7312bb288d453f5391705a38f330c305f4c2b4c8f9e4e1156ef0eb33d8" },
                { "si", "c439b1551cb695b59adf4ebab4eb6e0819e3eabc47f0f508bc24dd720e081986f2c814bf2e0b1a7d287fb99925dffd0bd3702d51906d310b3a9637ff58862554" },
                { "sk", "b16f2d39320933efc76ebfa5b19e448ff97feed1c48be17078d48fadeafe2c91e000e9481f558452405a31108929a00249eb58958841dc3d0a2b6a1caba69fda" },
                { "skr", "eeff3a6a2c8fee5cd257b160e7a60d7dcbe0dec10d045cef7f7321bae4cea0ee2566a1a14d675bc04606d951e7cdb937e07d518caec3ccdcb61e1e8ad5e0a9e2" },
                { "sl", "a0c72a9fd1bb57f0ffba11877044b705e4dc99b04de9943a9aeda5936764c2a0b0df73c2007cf32138cacaa0c4177aaa4e3f36e52e775e5c045eaad58cf261f9" },
                { "son", "3f514137b625ce35665cadf18bcf59fb87928da49d082dcc5f5cc2d8c0964396282eb79604883d5902f3bbf4d08dc654281a474f734001272e17f02167883f69" },
                { "sq", "8df964d2fc1dd704ff5f8eb0d3204152d74be93198a2bd4f128cf2b5af70beddc6c8d9df7af4715ba5e1e4ba05720412a12d5ba3208b6a7f3c19d6b1e9b6a865" },
                { "sr", "10a75fd4316819af1b7e67fb7efeca1aef77506d73e6a0189caf38da2ded1283fed0b45198613dc00f9a743508c9708620d23adabb745d368b416a2c6c76283e" },
                { "sv-SE", "4596b5976be84e1a424f020e8312c3b02133b2fd6f00dcae24439a5ca0b69c531b47e392540dca5abea2b8638c5739de85a3fc41159a6613031714e5871aca46" },
                { "szl", "50e8b33f0d46d8531f32b2843d82325bf9d2644563eb57b3f6a1148f2d4d859c067e89707baa03a405e2af1d81e80346f37e119d0b462c4a34b96b1d4abdebfe" },
                { "ta", "f14473790e05dfba0b30ad08fc0f00a49fa5d9572bc25f1591015ce75b717ded286c5b209baac2cdb7d369c24a185e8af851b1c1d770f64a0db205a072ed0392" },
                { "te", "76ba62d232b8248c9ff9844bbd40392deb38af11b7c9cd8a5777b6dc201f2663bf3666fdf6fce5ea300a129b247e82872dc9203d1b965578b11a892569fa78c0" },
                { "tg", "69db38b3a4ae9702da9fd37d9f4d8eb056ca0a3206e6e38087a7c0702f109a336a9e5458a5454c02269dad4ef17fdd0c95074a6db35fec1ead91b0bdefc7e45f" },
                { "th", "7092a13c5180647e93f48e9c98bc8963eee68eaad3e5d262d00e6fface57352c42f32e814aea0c900943f2acd06f7929fa8ffe96f88b350a357b407702414713" },
                { "tl", "c2fdddb764d76592c227a6936a9c45b1895ae4eb90d78ce2277d0b59ffbec94dc843cc8b835536cb3161898276e4e40dd3dc6b7fdc40e6d8b8447dfbd77bd712" },
                { "tr", "eff4b5c2ad1102dc80a2161bd0e6181c56742060c48fa88a6d82074db1b269e4c3087e7c3da527c50c8c7dc8965eea836477b2f40dcc65ed6c094385416e43ac" },
                { "trs", "8e4478604cf7f62104c08981bd36b27fd273ee7dabbbd1bc188ddba5b864b2903ecc23379816b51267f72b69c8346edc8c280d5e27b0a60b4def1b30ab0aeb02" },
                { "uk", "81bf285a4003722749074eda787bdf128c65f69a7aac7fc09a892fe33e41556629c6e3ec449d40a5a106562c90c764f22845ba9a5f176e955e1e3bfafdd0e3d9" },
                { "ur", "6bef48807a91107ee90d53565823f6baf53a3385a2665f20bc65e672d41bde2357aee06a45f62e8090204f670cee5bcd7fea2d80d55176aea5885cb89575960e" },
                { "uz", "09a52f018376bed2b0c61692ca9fc55f9ded4689aaf8bc2d384f31436f998da39224d5f3e3608db55762a0a5f260c8b7b643f5f934d1b469dbbec6600bf071ce" },
                { "vi", "9acbe8f949516d1f5f38b3389d6db02be1db63c0dd4a4b6718027a25f9f28b219a339645a3aa2735ae50d99dbc3d301c53ba257f2d61beddcc9e5a535b8eaafc" },
                { "xh", "920951e447cf5adec1372578ec21dce38571a260a3294184efafe670fc323745b4da661f7c07c939c6b5c6c2050ee71a1c3ab19fe2ec124a00486705d2e77f5f" },
                { "zh-CN", "d97fbf91afd7571832640e24358b1bc047a52a7858aa65e26b97039889523ef92bbbe7f34d54dc814ea335ca0b5f78d23b4499a8eb0704e1b172885a92b43f46" },
                { "zh-TW", "f0203db433aacaa3fd2f812083efa9c88e0cb74b7a505b28d99cbaed4238aa227cba7a918ee803f69a317c3a9a1b9e73cd91d57d1eb1e15f6faa496674cd214b" }
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
